using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4.0f;
    private Player _player;
    private Animator _anim;
    [SerializeField]
    private AudioClip _enemyExplodingSoundClip;
    [SerializeField]
    private AudioSource[] _audioSource;
    [SerializeField] 
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate;
    private float _canFire = -1.0f;
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponents<AudioSource>();
        
        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }
        
        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Enemy prefab doesn't have audio source component!");
        }
        else
        {
            //_audioSource.clip = _enemyExplodingSoundClip;
        }
        
    }

    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3.0f, 6.0f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            _audioSource[1].Play();
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
               lasers[i].AssignEnemyLaser(); 
            }
            
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Time.deltaTime * _enemySpeed * Vector3.down);
        
        // if enemy object is at the bottom of screen
        // respawn at top with a random X-Axis position
        if (transform.position.y <= -10.5f)
        { 
            float randomX = Random.Range(-9.46f,9.46f);
            transform.position = new Vector3(randomX, 10.5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
            }
            
            _anim.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0;
            _audioSource[0].Play(); 
            Destroy(this.gameObject, 2.2f);
        }
        
        if (other.CompareTag("Laser"))
        {   
            Destroy(other.gameObject);     
            if (_player != null)
            {
                _player.IncreaseScore(10);
            }
            
            _anim.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0;
            _audioSource[0].Play();
            
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.2f);
        }
    }
    
}

//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseBehavior : MonoBehaviour
{
    
    public enum State { PATROL, ATTACK, SEARCH, RETURN, STUNNED, DEAD}
    [Header("States")]
    [SerializeField]
    public State state = State.PATROL;

    public enum Facing { NORTH, SOUTH, EAST, WEST};
    [SerializeField]
    public Facing facing = Facing.NORTH;

    Vector3 startPos;


    [Header("Movement")]
    public bool cutscene;
    public int speed;
    public PlayerController player;
    private Vector3 destination;
    private Vector3 nextTile;
    private Vector3 lastSeen;
    private bool gotToLastSeen = false;
    [Header("Search")]
    public float veiwAngle = 45;
    //also the spot distance
    public float lossDist = 10;
    public float searchTime = 30;
    public float stunTime = 5;
    public float timeLeft;
    public float wanderDistance;
    public float MaxDistFromHome;
    
    Animator animator;
    SpriteRenderer spRenderer;
    BoxCollider2D boxCollider;
    ParticleSystem blood;
    AudioSource sounds;
    MenuScript menu;

    [System.Serializable]
    public struct DropChance
    {
        public Item dropItem;
        [Range(0.0f, 1.0f)]
        public float chance;
    }

    [Header("Item Drop")]
    [SerializeField]
    public DropChance itemDrop;

    public GameObject itemBase;

    [Header("Sounds")]
    public AudioClip footSteps;
    public AudioClip death;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        animator = GetComponent<Animator>();
        spRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        blood = GetComponent<ParticleSystem>();
        sounds = GetComponent<AudioSource>();
        destination = transform.position;
        startPos = transform.position;
        menu = FindObjectOfType<MenuScript>();
        //Vector2 tileLoc = PathFinder.FindPath(new Vector2(transform.position.x, transform.position.y),
        //                               new Vector2(destination.x, destination.y))[1];
        //nextTile = PathFinder.map.getTilePosition((int)tileLoc.x, (int)tileLoc.y);
        //nextTile += new Vector3(0.5f, 0.5f, 0f);
        nextTile = transform.position;
    }

    private void OnEnable()
    {
        menu = FindObjectOfType<MenuScript>();
        FindObjectOfType<MapGeneration>().mapGenerated += MapGenerated;
        menu.onMenuChange += OpenMenu;
        FindObjectOfType<DialogSystem>().onComplete += TextComplete;
        FindObjectOfType<DialogSystem>().onStart += TextStart;
    }

    private void OnDisable()
    {
        FindObjectOfType<MapGeneration>().mapGenerated -= MapGenerated;
        menu.onMenuChange -= OpenMenu;
        FindObjectOfType<DialogSystem>().onComplete -= TextComplete;
        FindObjectOfType<DialogSystem>().onStart -= TextStart;
    }

    // Update is called once per frame
    void Update()
    {
        if(state != State.DEAD && !cutscene)
        {
            int layermask = 1 << 9;
            layermask = ~layermask;
            RaycastHit2D hitInfo;
            switch (state)
            {
                case State.PATROL:

                    Debug.DrawLine(transform.position, player.transform.position, Color.blue);
                    hitInfo = Physics2D.Linecast(transform.position, player.transform.position, layermask);
                    if (hitInfo.collider != null)
                        if (hitInfo.distance < lossDist && hitInfo.collider.gameObject.tag == "Player")
                        {
                            Vector3 angle = player.transform.position - transform.position;
                            if (Vector3.Angle(getDirection(), angle) < veiwAngle / 2)
                            {
                                spRenderer.color = Color.red;
                                state = State.ATTACK;
                            }
                        }
                    if (Vector3.Distance(transform.position, startPos) > MaxDistFromHome)
                    {
                        spRenderer.color = Color.white;
                        state = State.RETURN;
                    }
                    break;

                case State.ATTACK:
                    //if looses sight of player go to last known position and then wander
                    Debug.DrawLine(transform.position, player.transform.position, Color.red);
                    hitInfo = Physics2D.Linecast(transform.position, player.transform.position, layermask);
                    if (hitInfo.collider != null)
                    {
                        if (hitInfo.collider.gameObject.tag == "Player")
                        {
                            Debug.Log("Player sighted");
                        }
                        else
                        {
                            spRenderer.color = Color.white;
                            state = State.SEARCH;
                            lastSeen = PathFinder.map.getClosestTile(player.transform.position);
                            timeLeft = searchTime;
                            gotToLastSeen = false;
                        }
                    }
                    else
                    {
                        spRenderer.color = Color.white;
                        state = State.SEARCH;
                        lastSeen = PathFinder.map.getClosestTile(player.transform.position);
                        timeLeft = searchTime;
                        gotToLastSeen = false;
                    }
                    break;

                case State.SEARCH:
                    //Start a timer, if timer end goto return
                    //if player is spotted in the elasped time return to attacking
                    Debug.DrawLine(transform.position, player.transform.position, Color.blue);
                    hitInfo = Physics2D.Linecast(transform.position, player.transform.position, layermask);
                    if (hitInfo.collider != null)
                        if (hitInfo.distance < lossDist && hitInfo.collider.gameObject.tag == "Player")
                        {
                            Vector3 angle = player.transform.position - transform.position;
                            if (Vector3.Angle(getDirection(), angle) < veiwAngle / 2)
                            {
                                spRenderer.color = Color.red;
                                state = State.ATTACK;
                            }
                        }
                    if (gotToLastSeen)
                    {
                        if (timeLeft <= 0)
                        {
                            spRenderer.color = Color.white;
                            state = State.RETURN;
                        }
                        else
                        {
                            timeLeft -= Time.deltaTime;
                        }
                    }

                    break;

                case State.RETURN:
                    Debug.DrawLine(transform.position, player.transform.position, Color.blue);
                    hitInfo = Physics2D.Linecast(transform.position, player.transform.position, layermask);
                    if (hitInfo.collider != null)
                        if (hitInfo.distance < lossDist && hitInfo.collider.gameObject.tag == "Player")
                        {
                            Vector3 angle = player.transform.position - transform.position;
                            if (Vector3.Angle(getDirection(), angle) < veiwAngle / 2)
                            {
                                spRenderer.color = Color.red;
                                state = State.ATTACK;

                            }
                        }
                    if (Vector3.Distance(transform.position, startPos) < 0.001f)
                    {
                        transform.position = startPos;
                        spRenderer.color = Color.white;
                        state = State.PATROL;
                    }
                    break;

                case State.STUNNED:
                    if (timeLeft <= 0)
                    {
                        spRenderer.color = Color.red;
                        state = State.ATTACK;
                    }
                    else
                    {
                        timeLeft -= Time.deltaTime;
                    }
                    break;
            }

            //move to next tile
            if (Vector3.Distance(transform.position, nextTile) < 0.001f)
            {
                switch (state)
                {
                    case State.PATROL:
                        if (Vector3.Distance(transform.position, destination) < 0.001f)
                            destination = Wander();
                        break;
                    case State.ATTACK:
                        destination = player.transform.position;
                        break;
                    case State.SEARCH:
                        if (gotToLastSeen)
                        {
                            if (Vector3.Distance(transform.position, destination) < 0.001f)
                            {
                                destination = Wander();
                            }
                        }
                        else
                        {
                            if (Vector3.Distance(transform.position, lastSeen) < 0.001f)
                            {
                                gotToLastSeen = true;
                                destination = Wander();
                            }
                            else
                                destination = lastSeen;
                        }

                        break;
                    case State.RETURN:
                        destination = startPos;
                        break;
                    case State.STUNNED:
                        destination = transform.position;
                        break;

                }

                transform.position = nextTile;
                //find next tile and update destenation

                List<Vector2> tileLoc = PathFinder.FindPath(new Vector2(transform.position.x, transform.position.y),
                                                            new Vector2(destination.x, destination.y));
                if (tileLoc != null)
                {
                    if (tileLoc.Count > 1)
                        nextTile = PathFinder.map.getTilePosition((int)tileLoc[1].x, (int)tileLoc[1].y);
                }
                else
                    nextTile = transform.position;

                facing = GetFacing();
            }
            else
            {
                //move towards tile
                float step = speed * Time.deltaTime;

                transform.position = Vector3.MoveTowards(transform.position, nextTile, step);
            }
        }
    }

    public Facing GetFacing()
    {
        Vector3 direction = (nextTile - transform.position).normalized;
        animator.SetFloat("XVeloc", direction.x);
        animator.SetFloat("YVeloc", direction.y);
        if (direction.y < -0.5f)
        {
            animator.SetBool("North", false);
            animator.SetBool("Side", false);
            return Facing.SOUTH;
        }
        else if(direction.y > 0.5f)
        {
            animator.SetBool("North", true);
            animator.SetBool("Side", false);
            return Facing.NORTH;
        }
        else if(direction.x < -0.5f)
        {
            animator.SetBool("Side", true);
            spRenderer.flipX = false;
            return Facing.WEST;
        }
        else if(direction.x > 0.5f)
        {
            animator.SetBool("Side", true);
            spRenderer.flipX = true;
            return Facing.EAST;
        }
        else
        {
            return facing;
        }
    }

    public Vector3 getDirection()
    {
        switch (facing)
        {
            case Facing.NORTH:
                return Vector3.up;
            case Facing.SOUTH:
                return Vector3.down;
            case Facing.WEST:
                return Vector3.left;
            case Facing.EAST:
                return Vector3.right;
            default:
                return Vector3.zero;
        }
    }

    public Vector3 Wander()
    {
        Vector2 tileOn = PathFinder.map.getTileFromPosition(transform.position.x,
                                                            transform.position.y);
        Vector2 randomTile = tileOn + new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
        while (true)
        {
            if (PathFinder.map.getTileTypeFromTilePosition((int)randomTile.x, (int)randomTile.y) == MapGeneration.TileType.Ground)
                break;
            randomTile = tileOn + new Vector2(Random.Range(-wanderDistance, wanderDistance), 
                                              Random.Range(-wanderDistance, wanderDistance));
        }
        return PathFinder.map.getTilePosition((int)randomTile.x, (int)randomTile.y);
    }

    public void Kill()
    {
        state = State.DEAD;

        //Replace with play kill animation
        spRenderer.enabled = false;
        //remove the collider so the player doesn't kill themself on a dead body.
        boxCollider.enabled = false;

        blood.Play();

        sounds.PlayOneShot(death);

        float i = Random.Range(0.0f, 1.0f);
        if(itemDrop.dropItem != null)
        {
            if(i < itemDrop.chance)
            {
                Vector2 coord = PathFinder.map.getTileFromPosition(transform.position.x,
                                                                   transform.position.y);
                GameObject a = Instantiate(itemBase);
                a.GetComponent<WorldItem>().ItemFile = itemDrop.dropItem;
                a.GetComponent<WorldItem>().amount = 1;

            }
        }
    }

    public void Stun()
    {
        timeLeft = 5;
        state = State.STUNNED;

        //Figure something out
        spRenderer.color = Color.yellow;

    }

    public void MapGenerated()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void OpenMenu(bool status)
    {
        cutscene = status;
    }


    private void TextComplete()
    {
        cutscene = false;
    }


    private void TextStart()
    {
        cutscene = true;
    }


}

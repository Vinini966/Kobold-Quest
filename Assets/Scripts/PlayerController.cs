//code by Vincent Kyne and Jonathan Baptiste

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    

    Rigidbody2D playerBody;
    float horizontalInput;
    float verticalInput;
    public float speed = 10;
    int halfSpeed = 1;
    float normalizeSpeed = 0.82f;
    bool dead = false;

    float fadeoutTime = 2;

    public Inventory inventory;
    public MapGeneration map;
    public MenuScript menu;

    Animator animator;
    SpriteRenderer spRenderer;
    PlayerStats playerStats;
    
    public enum Facing { NORTH, SOUTH, EAST, WEST};
    [SerializeField]
    public Facing facing = Facing.NORTH;

    public bool cutScene = false;
    public bool usingItem = false;
    public Timer useTimer;
    public Slider timeBar;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        spRenderer = gameObject.GetComponent<SpriteRenderer>();
        map = FindObjectOfType<MapGeneration>(); //there should only be one in a scene.
        menu = FindObjectOfType<MenuScript>();
        playerStats = PlayerStats.getInstacne();
    }

    private void OnEnable()
    {
        menu = FindObjectOfType<MenuScript>();
        menu.onMenuChange += OpenMenu;
    }

    private void OnDisable()
    {
        menu.onMenuChange -= OpenMenu;
    }

    private void Update()
    {
        if (!cutScene && !dead)
        {
            if (inventory != null)
            {
                
                    horizontalInput = Input.GetAxisRaw("Horizontal");
                    verticalInput = Input.GetAxisRaw("Vertical");
                    if (Input.GetKey(KeyCode.LeftShift) || inventory.open)
                        halfSpeed = 2;
                    else
                        halfSpeed = 1;

                if (!inventory.open)//rplace with menu is open later
                {
                    if (Input.GetKeyUp(KeyCode.Q))
                    {
                        if (inventory.totalTraps > 0)
                            inventory.trapOn = Mathf.Clamp(inventory.trapOn - 1, 0, inventory.totalTraps);
                    }
                    else if (Input.GetKeyUp(KeyCode.E))
                    {
                        if (inventory.totalTraps > 0)
                            inventory.trapOn = Mathf.Clamp(inventory.trapOn + 1, 0, inventory.totalTraps-1);
                    }  
                    else if (Input.GetKeyUp(KeyCode.Space) && playerStats.traps.Count != 0)
                    {
                        if (!usingItem)
                        {
                            Vector2 coord = map.getTileFromPosition(transform.position.x, transform.position.y);
                            if (map.getTileTypeFromTilePosition((int)coord.x, (int)coord.y) == MapGeneration.TileType.Ground)
                            {
                                usingItem = true;
                                useTimer = new Timer(playerStats.traps[inventory.trapOn].useTime);
                                useTimer.StartTimer();
                                timeBar.gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            usingItem = false;
                            useTimer.StopTimer();
                            timeBar.gameObject.SetActive(false);
                        }
                        
                    }
                    else if (Input.GetKeyUp(KeyCode.Escape))
                    {
                        menu.Settings();
                        cutScene = true;
                    }
                }
                if (Input.GetKeyUp(KeyCode.I))
                {
                    if (!inventory.open)
                        inventory.Show();
                    else
                        inventory.Hide();
                }
                if (Input.GetKeyUp(KeyCode.C))
                {
                    if (!inventory.open)
                        inventory.ShowCrafting();
                    else
                    {
                        if(inventory.crafting)
                            inventory.Hide();
                        else
                            inventory.ShowCrafting();
                    }
                        
                }
            }
            else
                Debug.LogError("Inventory not connected to Player. Where is Inventory?");
        }
        else
        {
            horizontalInput = 0;
            verticalInput = 0;
            if (dead)
            {
                if(fadeoutTime <= 0)
                {
                    
                    Destroy(PlayerStats.getInstacne().gameObject);
                    gameObject.GetComponent<ChangeScene>().changeSceneTo("GameOver");
                }
                else
                {
                    fadeoutTime -= Time.deltaTime;
                }
            }
        }
            
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (usingItem && !menu.isOpen)
        {
            if (useTimer.CheckTimer())
            {
                Vector2 coord = map.getTileFromPosition(transform.position.x, transform.position.y);
                GameObject a = Instantiate(inventory.trapPrefab);
                Vector2 tilePos = map.getTilePosition((int)coord.x, (int)coord.y);
                a.transform.position = new Vector2(tilePos.x, tilePos.y);
                a.GetComponent<TrapCollisionEvent>().item = playerStats.traps[inventory.trapOn];
                a.GetComponent<TrapCollisionEvent>().UpdateData();
                inventory.TrashItem(playerStats.traps[inventory.trapOn], 1);
                usingItem = false;
                useTimer.StopTimer();
                timeBar.gameObject.SetActive(false);
            }
            else
            {
                useTimer.TimerUpdate();
                timeBar.value = useTimer.GetPercent();
            }
        }
        else
        {
            animator.SetFloat("XVeloc", horizontalInput);
            animator.SetFloat("YVeloc", verticalInput);
            facing = GetFacing();
            if (horizontalInput != 0 && verticalInput != 0)
            {
                horizontalInput *= normalizeSpeed;
                verticalInput *= normalizeSpeed;
            }

            playerBody.velocity = new Vector2(horizontalInput * speed / halfSpeed, verticalInput * speed / halfSpeed);
        }

        
    }

    public Facing GetFacing()
    {
        if (verticalInput < -0.1f)
        {
            animator.SetBool("North", false);
            animator.SetBool("Side", false);
            return Facing.SOUTH;
        }
        else if (verticalInput > 0.5f)
        {
            animator.SetBool("North", true);
            animator.SetBool("Side", false);
            return Facing.NORTH;
        }
        else if (horizontalInput < -0.5f)
        {
            animator.SetBool("Side", true);
            spRenderer.flipX = false;
            return Facing.WEST;
        }
        else if (horizontalInput > 0.5f)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Enemy" &&  !dead)
        {
            //GAME OVER
            animator.SetTrigger("Die");
            collision.gameObject.GetComponent<Animator>().SetTrigger("Attack");
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            playerStats.currentLevel = 0;
            playerStats.maxRooms = 20;
            playerStats.maxEnemies = 5;
            dead = true;
        }
    }

    void OpenMenu(bool status)
    {
        cutScene = status;
    }
}

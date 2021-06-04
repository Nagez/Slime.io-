using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
 
    public GameObject Player;
    public GameObject GameControl;
    public Transform[] StartRock;
    public Transform[] MainPath;

    public int slimeLevel = 1;
    public int PlayerPosition = 0;
    public bool FirstRollMove = true;
    
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] public int DiceNum = 0;

    
    public bool moveAllowed = false;

    [SerializeField] ParticleSystem smokeParticles;
    [SerializeField] ParticleSystem starsParticles;

    public Animator anim; 

    // Start is called before the first frame update
    void Start()
    {
        transform.position = StartRock[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveAllowed)
        {
            SlimeMovment(DiceNum);
        }   
    }
    
    public void CollisionChecking()
    {
        Rigidbody2D CurrentSlime = GetComponent<Rigidbody2D>();
        
        for (int i=0;i< 4 ; i++)//add mex players
        {
            GameObject GameControlsP = GameObject.Find("GameControls");//GetComponent<GameControl>().Players[i];
            GameObject PlayerS = GameControlsP.GetComponent<GameControl>().Players[i];

            for (int j=0;j< PlayerS.GetComponent<PlayerScript>().Slimes.Count ; j++)
            {
                GameObject testSlime = PlayerS.GetComponent<PlayerScript>().Slimes[j];

                if(CurrentSlime.GetComponent<Slime>().PlayerPosition == testSlime.GetComponent<Slime>().PlayerPosition)
                {
                    if (CurrentSlime.GetComponentInParent<PlayerScript>().playerNum == testSlime.GetComponentInParent<PlayerScript>().playerNum)
                    {
                        if(CurrentSlime.name == testSlime.name)
                        {
                            
                        }
                        else
                        {
                            Debug.Log("levelUp "+CurrentSlime.name);
                            LevelUP(CurrentSlime, testSlime);
                            CurrentSlime.GetComponent<Slime>().starsParticles.Play();

                            Debug.Log("Destroy " + testSlime.name);

                            
                            testSlime.GetComponentInParent<PlayerScript>().SlimeSpawnNeeded();
                            //SlimeSpawnNeeded();



                            Destroy(testSlime);
                            


                            PlayerS.GetComponent<PlayerScript>().Slimes.RemoveAt(j);
                            break;
                        }
                        
                    }
                    else
                    {
                        
                        Debug.Log("Destroy "+ testSlime.name);
                        CurrentSlime.GetComponent<Slime>().smokeParticles.Play();
                        
                        testSlime.GetComponentInParent<PlayerScript>().SlimesSpawned -= testSlime.GetComponent<Slime>().slimeLevel;

                        testSlime.GetComponentInParent<PlayerScript>().SlimeSpawnNeeded();
                        
                        Destroy(testSlime);
                        
                        //add cube if no 5 cubes
                        PlayerS.GetComponent<PlayerScript>().Slimes.RemoveAt(j);

                        //
                        //addCubeAfterDe();
                        break;
                    }
                }
            }
        }
    }


    //////FIXED FUNCS
    /////New Slime Generator
    public void InitNewSlime(int playerNum, int slimeNumber,int TotalSlimesSpawned)
    {
        GameObject PlayerT = GameObject.Find("Player" + playerNum);
        transform.parent = PlayerT.transform;
        PlayerPosition = 0;
        slimeLevel = 1;
        DiceNum = 0;
        moveAllowed = false;
        int countN = PlayerT.GetComponent<PlayerScript>().Slimes.Count;//NewSlime.GetComponentsInParent<PlayerScript>().Length;
        //Change name
        name = name.Replace("(Clone)", "");
        name = System.Text.RegularExpressions.Regex.Replace(name, @"[\d-]", string.Empty);
        name += TotalSlimesSpawned; 
        
    }

    //Slime selection for movment
    private void OnMouseDown()
    {
        if (GetComponentInParent<PlayerScript>().PTurn && !(GameControl.GetComponent<GameControl>().DicePICKED == 0))
        {
            DiceNum = GameControl.GetComponent<GameControl>().DicePICKED;
            GameControl.GetComponent<GameControl>().DicePICKED = 0;
            moveAllowed = true;
            //GameControl.GetComponent<GameControl>().DicePICKED = 0;
        }

    }

    //Slime Fusion
    public void LevelUP(Rigidbody2D CurrentSlime, GameObject testSlime)
    {
        Debug.Log(CurrentSlime.GetComponent<Slime>().slimeLevel + "+" + testSlime.GetComponent<Slime>().slimeLevel);
        int addLevel = CurrentSlime.GetComponent<Slime>().slimeLevel + testSlime.GetComponent<Slime>().slimeLevel;
        Debug.Log("=" + addLevel);
        CurrentSlime.GetComponent<Slime>().slimeLevel = addLevel;
        anim.SetInteger("Level", addLevel);
    }

    //slime movment
    public void SlimeMovment(int DiceRoll)
    {
        if (PlayerPosition == 31) //end route
        {
            Player.GetComponent<PlayerScript>().SlimesLeft -= slimeLevel;
            Player.GetComponent<PlayerScript>().SlimesSpawned -= slimeLevel;
            GameControl.GetComponent<GameControl>().UpdatePlayerLivesHud(); // update lives at hud
            GameControl.GetComponent<GameControl>().GameOverFunc();
            Player.GetComponent<PlayerScript>().Slimes.Remove(this.gameObject);
            Destroy(this.gameObject); 
            Destroy(this);
            Debug.Log("Slime finished route");
        }

        if (DiceRoll > 0)
        {
            int newRock = PlayerPosition + 1;

            Rigidbody2D SlimeMovment = GetComponent<Rigidbody2D>();

            try { transform.position = Vector3.MoveTowards(transform.position, MainPath[newRock].transform.position, moveSpeed * Time.deltaTime); }
            catch (System.IndexOutOfRangeException e)
            { Debug.Log("boom"+e);
                int ArrP = GameControl.GetComponent<GameControl>().DicePICKEDArr;
                GameControl.GetComponent<GameControl>().DiceMoves[ArrP] = 0;
            }
            

            if ((SlimeMovment.position.x == transform.position.x) && (SlimeMovment.position.y == transform.position.y))
            {
                //Player.GetComponent<PlayerScript>().DiceMoves[0]--;
                DiceNum--;
                PlayerPosition++;

                //if (PlayerPosition == 31)
                //{
                //    Player.GetComponent<PlayerScript>().SlimesLeft -= slimeLevel;
                //    Destroy(SlimeMovment);
                //}


                //First Path
                //out
                if ((PlayerPosition == 5) && (DiceNum == 0))
                {
                    PlayerPosition = 19;

                }

                //Secend Path
                //out
                else if ((PlayerPosition == 10) && (DiceNum == 0))
                {
                    PlayerPosition = 24;
                }

                //Third Path

                //in
                else if ((PlayerPosition == 24))
                {
                    PlayerPosition = 14;
                }

                //Fourth Path
                //out
                else if ((PlayerPosition == 22) && (DiceNum == 0))
                {
                    PlayerPosition = 27;
                }

                //End
                else if ((PlayerPosition == 19) || (PlayerPosition == 29))
                {
                    PlayerPosition = 29;
                }

            }

        }
        if (DiceRoll == 0)
        {
            if (Player.GetComponent<PlayerScript>().FirstMove)
                DiceNum--;
            moveAllowed = false;
            CollisionChecking();
            int ArrP= GameControl.GetComponent<GameControl>().DicePICKEDArr;
            GameControl.GetComponent<GameControl>().DiceMoves[ArrP] = 0;
            UpdateDiceInGameControl();
       
            GameControl.GetComponent<GameControl>().firstDiceThrown = true;
            Player.GetComponent<PlayerScript>().FirstMove = true;
        }
        //if (DiceRoll == -1)
        //{
        //    CollisionChecking();
        //    moveAllowed = false;
        //    GameControl.GetComponent<GameControl>().DiceMoves[0] = -1;
        //    Player.GetComponent<PlayerScript>().FirstMove = true;
        //}

        FirstRollMove = false;
    }

    //Update Dice In Game
    void UpdateDiceInGameControl()
    {
        GameObject GameAB = GameObject.Find("GameControls");
        for (int i = 0; i < GameAB.GetComponent<GameControl>().DiceMoves.Length; i++)
        {
            if ((GameAB.GetComponent<GameControl>().DiceMoves[i] == 0) && (i != (GameAB.GetComponent<GameControl>().DiceMoves.Length - 1)))
            {
                int T = GameAB.GetComponent<GameControl>().DiceMoves[i];
                GameAB.GetComponent<GameControl>().DiceMoves[i] = GameAB.GetComponent<GameControl>().DiceMoves[i + 1];
                GameAB.GetComponent<GameControl>().DiceMoves[i + 1] = T;
            }

        }
    }

    //add one more Cube
    void addCubeAfterDe()
    {
        GameControl.GetComponent<GameControl>().firstDiceThrown = false;
        GameObject.Find("Dice 1").GetComponent<CubeScript>().StartCoroutine("DiceRollImagegg");
        GameControl.GetComponent<GameControl>().firstDiceThrown = true;
    }
}

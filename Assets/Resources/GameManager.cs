using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour{
    [System.Serializable]

    public class Card
    {
        public int ID;
        public GameObject Card_Piece;
        public bool Rotate;
        
        public Card(int Index, GameObject Gobj,bool _Rotate){
            ID=Index;
            Card_Piece=Gobj;
            Rotate = _Rotate;

            Card_Piece.GetComponent<Renderer>().material.SetTexture("_MainTex",Resources.Load<Texture2D>("Game_cards/Card_"+ID));
        }
    }

    public List<Card> AllCards = new List<Card>();
    public List<Card> SelectCards = new List<Card>();
    public Card TheCardILastClicked;

    public GameObject Card_Prefab;

    public Vector2 Grid;

    public float SpeedRot;
    public float Timer,TimeCap;
    public bool Adjust;

    void Start(){
        InitalizeCards();
    }

    void Update(){
        CardRotation();

        if(Adjust){
            Timer+=Time.deltaTime;
            if(Timer>TimeCap){
                RotationAdjustment();
                Timer=0;
                Adjust=false;
            }
        }
        GamePlay();
    }
    
    public void CardRotation(){
        for(int i=0; i<AllCards.Count; i++){
            if(AllCards[i].Rotate==true){
                if(AllCards[i].Card_Piece != null){
                    AllCards[i].Card_Piece.transform.rotation=Quaternion.RotateTowards(AllCards[i].Card_Piece.transform.rotation,
                    Quaternion.Euler(0,0,0),SpeedRot*Time.deltaTime);
                }
            }
            else if (AllCards[i].Rotate==false){
                if(AllCards[i].Card_Piece != null){
                    AllCards[i].Card_Piece.transform.rotation=Quaternion.RotateTowards(AllCards[i].Card_Piece.transform.rotation,
                    Quaternion.Euler(0,180,0),SpeedRot*Time.deltaTime);
                }
            }
        }
    }
    public void GamePlay(){
        if (Input.GetMouseButtonDown(0)){
            RaycastHit2D HitInfo;
            HitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y,0)),Vector3.zero,Mathf.Infinity);
            
            if (HitInfo){
                if (HitInfo.transform.tag=="Card"&& SelectCards.Count<2){
                    TheCardILastClicked = GetCardByGameObject(HitInfo.transform.gameObject);

                    if(SelectCards.Contains(TheCardILastClicked)==false){
                        SelectCards.Add(TheCardILastClicked);
                        TheCardILastClicked.Rotate=true;
                    }
                    if(SelectCards.Count==2){
                        if(SelectCards[0].ID==SelectCards[1].ID){
                            Destroy(SelectCards[0].Card_Piece,3);
                            Destroy(SelectCards[1].Card_Piece,3);

                            SelectCards.Clear();
                        }
                        else if(SelectCards[0].ID != SelectCards[1].ID){
                            Adjust=true;
                        }
                    }
                }
            }
        }
    }

    public void RotationAdjustment(){
        SelectCards[0].Rotate=false;
        SelectCards[1].Rotate=false;

        SelectCards.Clear();
    }

    public Card GetCardByGameObject(GameObject HitInfoCollider){
        for(int i = 0; i < AllCards.Count; i++){
            if(HitInfoCollider==AllCards[i].Card_Piece){
                return AllCards[i];
            }
        }
        return null;
    }

    public void InitalizeCards(){
        for(int i=0; i < Grid.x; i++){
            for(int j=0; j < Grid.y; j++){
                
                GameObject NewCard = Instantiate(Card_Prefab, new Vector3(i,j,0), Quaternion.Euler(0,0,0), transform);

                int TempId = Random.Range(0,10);

                while(checkForRepeatingIds(TempId) == true){
                    TempId = Random.Range(0,10);
                }

                AllCards.Add(new Card(TempId, NewCard,false));
            }
        }
    }
    public bool checkForRepeatingIds(int TemporalID){
        int Counter = 0;
        for(int i=0; i < AllCards.Count; i++){
            if(TemporalID == AllCards[i].ID){
                Counter++;
            }
        }
        if(Counter==2){
            return true;
        }
        return false;
    }
}
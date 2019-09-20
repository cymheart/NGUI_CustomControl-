using CommonNS;
using GameAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{
    [ExecuteInEditMode]
    public class SuitSelecter : Control
    {
        UISprite btnDiamond;
        UISprite btnClub;
        UISprite btnHeart;
        UISprite btnSpade;

        UISprite sprDiamond;
        UISprite sprClub;
        UISprite sprHeart;
        UISprite sprSpade;

        [SerializeField, SetProperty("SelectedSuit")]
        Suits selectedSuit;
        public Suits SelectedSuit
        {
            get { return selectedSuit; }
            set
            {
                selectedSuit = value;
                GreyBtn();
                switch(selectedSuit)
                {
                    case Suits.Diamond:
                        btnDiamond.GetComponent<UIButton>().defaultColor = Color.white;
                        break;
                    case Suits.Club:
                        btnClub.GetComponent<UIButton>().defaultColor = Color.white;
                        break;
                    case Suits.Heart:
                        btnHeart.GetComponent<UIButton>().defaultColor = Color.white;
                        break;
                    case Suits.Spade:
                        btnSpade.GetComponent<UIButton>().defaultColor = Color.white;
                        break;
                }

                if (BindProcess != null)
                    BindProcess(this);
            }
        }

        [SerializeField, SetProperty("IsDisabled")]
        bool isDisabled = false;
        public bool IsDisabled
        {
            get
            {
                return isDisabled;
            }
            set
            {
                isDisabled = value;

                if (isDisabled)
                {
                    UIEventListener.Get(btnDiamond.gameObject).onClick = null;
                    UIEventListener.Get(btnClub.gameObject).onClick = null;
                    UIEventListener.Get(btnHeart.gameObject).onClick = null;
                    UIEventListener.Get(btnSpade.gameObject).onClick = null;
                }
                else
                {
                    UIEventListener.Get(btnDiamond.gameObject).onClick = Click;
                    UIEventListener.Get(btnClub.gameObject).onClick = Click;
                    UIEventListener.Get(btnHeart.gameObject).onClick = Click;
                    UIEventListener.Get(btnSpade.gameObject).onClick = Click;
                }

                if (BindProcess != null)
                    BindProcess(this);
            }
        }


        protected override void Awake()
        {
            base.Awake();

            btnDiamond = gameObject.transform.Find("BtnDiamond").GetComponent<UISprite>();
            btnClub = gameObject.transform.Find("BtnClub").GetComponent<UISprite>();
            btnHeart = gameObject.transform.Find("BtnHeart").GetComponent<UISprite>();
            btnSpade = gameObject.transform.Find("BtnSpade").GetComponent<UISprite>();
           
            sprDiamond = btnDiamond.transform.GetChild(0).GetComponent<UISprite>();
            sprClub = btnClub.transform.GetChild(0).GetComponent<UISprite>();
            sprHeart = btnHeart.transform.GetChild(0).GetComponent<UISprite>();
            sprSpade = btnSpade.transform.GetChild(0).GetComponent<UISprite>();

            UIEventListener.Get(btnDiamond.gameObject).onClick = Click;
            UIEventListener.Get(btnClub.gameObject).onClick = Click;
            UIEventListener.Get(btnHeart.gameObject).onClick = Click;
            UIEventListener.Get(btnSpade.gameObject).onClick = Click;

            GreyBtn();
        }

        void GreyBtn()
        {
            btnDiamond.GetComponent<UIButton>().defaultColor = new Color(1, 1, 1, 0.5f);
            btnClub.GetComponent<UIButton>().defaultColor = new Color(1, 1, 1, 0.5f);
            btnHeart.GetComponent<UIButton>().defaultColor = new Color(1, 1, 1, 0.5f);
            btnSpade.GetComponent<UIButton>().defaultColor = new Color(1, 1, 1, 0.5f);
        }

        void Click(GameObject go)
        {
            GreyBtn();

            if (go == btnDiamond.gameObject)
            {
                btnDiamond.GetComponent<UIButton>().defaultColor = Color.white;
                selectedSuit = Suits.Diamond;
            }
            else if (go == btnClub.gameObject)
            {
                btnClub.GetComponent<UIButton>().defaultColor = Color.white;
                selectedSuit = Suits.Club;
            }
            else if(go == btnHeart.gameObject)
            {
                btnHeart.GetComponent<UIButton>().defaultColor = Color.white;
                selectedSuit = Suits.Heart;
            }
            else if(go == btnSpade.gameObject)
            {
                btnSpade.GetComponent<UIButton>().defaultColor = Color.white;
                selectedSuit = Suits.Spade;
            }

            if (BindProcess != null)
                BindProcess(this);
        }

        protected override void Layout()
        {
            Vector3[] worldCorners = WorldCorners;
            float boxPosX = worldCorners[0].x;
            float y = (worldCorners[0].y + worldCorners[1].y) / 2;


            btnDiamond.height = Height;
            btnClub.height = Height;
            btnHeart.height = Height;
            btnSpade.height = Height;

            int n = (int)(Height * 0.72f);
            btnDiamond.width = n;
            btnClub.width = n;
            btnHeart.width = n;
            btnSpade.width = n;

            //
            n = (int)(Height * 0.7f);
            sprDiamond.height = n;
            sprClub.height = n;
            sprHeart.height = n;
            sprSpade.height = n;

            n = (int)(btnDiamond.width * 0.7f);
            sprDiamond.width = n;
            sprClub.width = n;
            sprHeart.width = n;
            sprSpade.width = n;

            int cardTotalWidth = btnDiamond.width + btnClub.width + btnHeart.width + btnSpade.width;
            float spacing = (Width - cardTotalWidth) / 3;
            float unitSpacing = spacing / scale;

            float unitCardWidth = btnDiamond.width / scale;
            float unitCardHalfWidth = unitCardWidth / 2;
            float x = boxPosX + unitCardHalfWidth;
            btnDiamond.transform.position = new Vector3(x, y, 0);

            x += unitSpacing + unitCardWidth;
            btnClub.transform.position = new Vector3(x, y, 0);

            x += unitSpacing + unitCardWidth;
            btnHeart.transform.position = new Vector3(x, y, 0);

            x += unitSpacing + unitCardWidth;
            btnSpade.transform.position = new Vector3(x, y, 0);
        }
    }
}
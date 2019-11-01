using CommonNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ControlNS
{

    [ExecuteInEditMode]
    public class CostInfo : Control
    {
        UILabel[] info = new UILabel[5];
        UISprite sprRoomCard;
        UISprite sprDiamond;
        public Font font;

        
        [SerializeField, SetProperty("RoomCardAmount")]
        int roomCardAmount = 0;
        public int RoomCardAmount
        {
            get { return roomCardAmount; }
            set
            {
                roomCardAmount = value;
                info[1].text = "[FFF08D] x" + roomCardAmount + "[-]";
                isReLayout = true;
                if (BindProcess != null)
                    BindProcess(this);
            }
        }

        [SerializeField, SetProperty("DiamondAmount")]
        int diamondAmount = 0;
        public int DiamondAmount
        {
            get { return diamondAmount; }
            set
            {
                diamondAmount = value;
                info[4].text = "[FFF08D]x" + diamondAmount + "[-]";
                isReLayout = true;
                if (BindProcess != null)
                    BindProcess(this);
            }
        }

        // <summary>
        /// 设置对齐方式
        /// </summary>
        /// <param name="alignment"></param>
        [SerializeField, SetProperty("Alignment")]
        int alignment = 1;
        public int Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                alignment = value;
                isReLayout = true;
            }
        }


        protected override void Awake()
        {
            base.Awake();

            info[0] = gameObject.transform.Find("info1").GetComponent<UILabel>();
            info[1] = gameObject.transform.Find("info2").GetComponent<UILabel>();
            info[2] = gameObject.transform.Find("info3").GetComponent<UILabel>();
            info[3] = gameObject.transform.Find("info4").GetComponent<UILabel>();
            info[4] = gameObject.transform.Find("info5").GetComponent<UILabel>();

            sprRoomCard = gameObject.transform.Find("SpriteRoomCard").GetComponent<UISprite>();
            sprDiamond = gameObject.transform.Find("SpriteDiamond").GetComponent<UISprite>();

            info[1].text = "[FFF08D] x" + roomCardAmount + "[-]";
            info[4].text = "[FFF08D]x" + diamondAmount + "[-]";
        }


        protected override void Layout()
        {   
            Vector3 eulerAngles = transform.localEulerAngles;
            transform.localEulerAngles = Vector3.zero;

          
            Vector2 size = CreateSizes(Height);
            float contentWidth = size.x;

            if(ctrlSizeChangeMode == ControlSizeChangeMode.FitContentSize)
            {
                Width = (int)contentWidth;
            }

            Vector3[] worldCorners = WorldCorners;
            float boxPosX = worldCorners[0].x;
            float y = (worldCorners[0].y + worldCorners[1].y) / 2;

            for (int i = 0; i < 5; i++)
                info[i].height = (int)size.y;

            sprRoomCard.height = (int)size.y;
            sprRoomCard.width = (int)size.y;

            sprDiamond.height = (int)(size.y * 1.5f);
            sprDiamond.width = sprDiamond.height;

            float startPosX = boxPosX;

            switch(alignment)
            {
                case 1:
                    {
                        float centerx = (worldCorners[0].x + worldCorners[2].x) / 2;
                        startPosX = (centerx - (contentWidth/scale) / 2);
                    }
                    break;
            }


            //
            float w = info[0].width / scale;
            info[0].transform.position = new Vector3(startPosX + w / 2, y, info[0].transform.position.z);

            startPosX += w;
            w = sprRoomCard.width / scale;
            sprRoomCard.transform.position = new Vector3(startPosX + w / 2, y, sprRoomCard.transform.position.z);

            startPosX += w;
            w = info[1].width / scale;
            info[1].transform.position = new Vector3(startPosX + w / 2, y, info[1].transform.position.z);

            startPosX += w;
            w = info[2].width / scale;
            info[2].transform.position = new Vector3(startPosX + w / 2, y, info[2].transform.position.z);

            startPosX += w;
            w = info[3].width / scale;
            info[3].transform.position = new Vector3(startPosX + w / 2, y, info[3].transform.position.z);

            startPosX += w;
            w = sprDiamond.width / scale;
            sprDiamond.transform.position = new Vector3(startPosX + w / 2, y, sprDiamond.transform.position.z);

            startPosX += w;
            w = info[4].width / scale;
            info[4].transform.position = new Vector3(startPosX + w / 2, y, info[4].transform.position.z);

            transform.localEulerAngles = eulerAngles;
        }


        Vector2 CreateSizes(int fontHeight)
        {
            int totalWidth;
            int[] widths = new int[5];
            string txt;
            int maxFontHeight = fontHeight;
            int minFontHeight = 0;
            int flag = 4;

            while (true)
            {
                totalWidth = fontHeight + (int)(fontHeight * 1.5f);

                for (int i = 0; i < 5; i++)
                {
                    if (i == 1)
                        txt = " x" + roomCardAmount;
                    else if (i == 4)
                        txt = "x" + diamondAmount;
                    else
                        txt = info[i].printedText;

                    widths[i] = GetTextRenderWidth(txt, info[i].trueTypeFont, fontHeight, info[i].fontStyle);
                    totalWidth += widths[i];
                }

                if (ctrlSizeChangeMode == ControlSizeChangeMode.FitContentSize ||
                    (totalWidth >= Width - 10 && totalWidth <= Width))
                {
                    for (int i = 0; i < 5; i++)
                        info[i].width = widths[i];

                    return new Vector2(totalWidth, fontHeight);
                }
                else if (totalWidth > Width)
                {
                    maxFontHeight = fontHeight;
                    fontHeight = (minFontHeight + maxFontHeight) / 2;
                }
                else if(totalWidth < Width)
                {
                    minFontHeight = fontHeight;
                    fontHeight = (minFontHeight + maxFontHeight) / 2;
                    flag--;
                }


                if(minFontHeight == maxFontHeight)
                {
                    for (int i = 0; i < 5; i++)
                        info[i].width = widths[i];

                    return new Vector2(totalWidth, fontHeight);
                }

                if (flag == 0)
                {
                    for (int i = 0; i < 5; i++)
                        info[i].width = widths[i];

                    return new Vector2(totalWidth, fontHeight);
                }
            }
        }

    }
}
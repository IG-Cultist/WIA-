/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EasingCore;
using KanKikuchi.AudioManager;

namespace FancyScrollView.Example09
{
    class Cell : FancyCell<ItemData>
    {
        readonly EasingFunction alphaEasing = Easing.Get(Ease.OutQuint);

        [SerializeField] Text title = default;
        [SerializeField] Text number = default;
        [SerializeField] Text date = default;
        [SerializeField] Text description = default;
        [SerializeField] Text quote = default;
        [SerializeField] RawImage image = default;
        [SerializeField] Image background = default;
        [SerializeField] CanvasGroup canvasGroup = default;

        [SerializeField] Texture titleTex = default;

        ItemData data;

        public override void UpdateContent(ItemData itemData)
        {
            data = itemData;
            image.texture = null;

            TextureLoader.Load(itemData.Url, result =>
            {
                if (image == null || result.Url != data.Url)
                {
                    return;
                }

                if (itemData.Number != 0) image.texture = result.Texture;

                else image.texture = titleTex;

            });

            //初期化
            title.text = "";
            number.text = "労働災害事案";
            date.text = "20XX年XX月XX日";
            description.text = "";
            quote.text = "引用元 : ";


            title.text = itemData.Title;
            if(itemData.Number != 0) number.text += itemData.Number.ToString();
            date.text = itemData.Date;
            description.text = itemData.Description;
            if (itemData.Quote != "") quote.text += itemData.Quote;

            UpdateSibling();
        }

        void UpdateSibling()
        {
            var cells = transform.parent.Cast<Transform>()
                .Select(t => t.GetComponent<Cell>())
                .Where(cell => cell.IsVisible);

            if (Index == cells.Min(x => x.Index))
            {
                transform.SetAsLastSibling();
            }

            if (Index == cells.Max(x => x.Index))
            {

                transform.SetAsFirstSibling();
            }

            //ページ送りSE
            SEManager.Instance.Play(
                audioPath: SEPath.NEXT_PAGE,   //再生したいオーディオのパス
                volumeRate: 1,                 //音量の倍率
                delay: 0,                      //再生されるまでの遅延時間
                pitch: 1,                      //ピッチ
                isLoop: false,                 //ループ再生するか
                callback: null                 //再生終了後の処理
            );

        }

        public override void UpdatePosition(float t)
        {
            const float popAngle = -15;
            const float slideAngle = 25;

            const float popSpan = 0.75f;
            const float slideSpan = 0.25f;

            t = 1f - t;

            var pop = Mathf.Min(popSpan, t) / popSpan;
            var slide = Mathf.Max(0, t - popSpan) / slideSpan;

            transform.localRotation = t < popSpan
                ? Quaternion.Euler(0, 0, popAngle * (1f - pop))
                : Quaternion.Euler(0, 0, slideAngle * slide);

            transform.localPosition = Vector3.left * 500f * slide;

            canvasGroup.alpha = alphaEasing(1f - slide);

            background.color = Color.Lerp(Color.gray, Color.white, pop);

            
        }
    }
}

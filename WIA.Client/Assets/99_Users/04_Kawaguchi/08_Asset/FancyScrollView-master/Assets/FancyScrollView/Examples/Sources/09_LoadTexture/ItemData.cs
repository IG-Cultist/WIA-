/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Example09
{
    class ItemData
    {
        public string Title { get; }       //事例タイトル
        public int Number { get; }      //事例連番(DBID)
        public string Date { get; }        //発生日時(デフォルトでも可)
        public string Description { get; } //災害説明文
        public string Quote { get; }       //引用元サイト
        public string Url { get; }         //画像URL

        public ItemData(string title, int number,string date, string description,string quote, string url)
        {
            Title = title;
            Number = number;
            Date = date;
            Description = description;
            Quote = quote;
            Url = url;
        }
    }
}

using UnityEngine;

namespace FancyScrollView.Example09
{
    class DictionaryManager : MonoBehaviour
    {
        /// <summary>
        /// ページ作成(DBから取得予定)
        /// </summary>
        readonly ItemData[] itemData =
        {
            new ItemData(
                "労働災害事例報告書",                       //見出し(事例名称)
                0,                                          //連番(データベースIDを流用)
                "20XX年XX月XX日",                           //事例発生日(あんまり記述されてないからデフォでもよき)
                "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",     //労災事例の詳細説明
                "",                                                    //URLか引用サイト名を記入
                "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/00.png"         //GitHubに格納した画像URLを記入
            ),
            new ItemData(
                "クライミングクレーンのジブ後方転倒",
                1,
                "20XX年XX月XX日",
                "建設工事現場に設置したクライミングクレーン（つり上げ荷重2.9トン）を用いて、型枠用パネルを吊り上げ、荷下ろし場所でジブを伏せていたところ、伏せ動作を止められなくなってジブが倒れ、中間部がビルに当たって折れ曲がった。このクレーンは、約30年前に製造され、様々な建設現場で、設置、稼働、解体が繰り返されてきた。ジブの伏せ動作が止まらなかった原因は、ジブ起伏用電動機の駆動軸の歯車の歯が折損し、歯がかみ合わずにドラムが空転してしまい、巻かれていた起伏用ワイヤーロープが抜けたことである。クレーンは、建設現場に設置される前に、巻き上げ装置や起伏用ドラムのブレーキライニングの摩耗などを確認しているものの、ドラム内部の歯車の状態までは確認していなかった。",
                "https://anzeninfo.mhlw.go.jp/anzen_pg/SAI_DET.aspx",
                "https://anzeninfo.mhlw.go.jp/anzen/sai/image/sai32/sai32-29-64-1-s.jpg"
            ),
            new ItemData(
                 "工事現場滑落事故",
                2,
                "20XX年XX月XX日",
                "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
                "https://引用元.com",
                "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/02.png"
            ),
            new ItemData(
                 "労働災害事例報告書",
                3,
                "20XX年XX月XX日",
                "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
                "https://引用元.com",
                "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/03.png"
            ),
            new ItemData(
               "労働災害事例報告書",
                4,
                "20XX年XX月XX日",
                "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
                "https://引用元.com",
                "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/04.png"
            ),
            new ItemData(
                "労働災害事例報告書",
                5,
                "20XX年XX月XX日",
                "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
                "https://引用元.com",
                "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/05.png"
            ),
            new ItemData(
                "労働災害事例報告書",
                6,
                "20XX年XX月XX日",
                "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
                "https://引用元.com",
                "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/06.png"
            ),
            new ItemData(
               "労働災害事例報告書",
                7,
                "20XX年XX月XX日",
                "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
                "https://引用元.com",
                "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/07.png"
            ),
            new ItemData(
                "労働災害事例報告書",
                8,
                "20XX年XX月XX日",
                "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
                "https://引用元.com",
                "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/08.png"
            ),
            new ItemData(
               "労働災害事例報告書",
                9,
                "20XX年XX月XX日",
                "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
                "https://引用元.com",
                "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/09.png"
            )
        };

        [SerializeField] ScrollView scrollView = default;

        void Start()
        {
            scrollView.UpdateData(itemData);
        }
    }
}

using UnityEngine;

namespace FancyScrollView.Example09
{
    class DictionaryManager : MonoBehaviour
    {
        [Header("遷移フェードカラー")]
        [SerializeField]
        Color32 endColor = new Color32(255, 255, 255, 255);

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
                "労働災害啓発協会",                                                    //URLか引用サイト名を記入
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
                 "高所作業中の転落事故",
                2,
                "20XX年XX月XX日",
                "高所での作業にて、建築に必要な資材を運搬中に足元不注意が原因で高さ15mのプラットフォームから落下。被災者の命に別状はなかったものの、全治7ヵ月もの重傷を負ってしまった。その後の聞き込みにて、命綱無しでの作業や、落下防止網の設置等、義務付けられている労災防止策を一切施していないことが判明した。",
                "労働災害啓発協会",
                "https://github.com/IG-Cultist/WIA-/blob/develop/image/fall.png?raw=true"
            ),
            new ItemData(
                 "業務外での労働災害",
                3,
                "20XX年XX月XX日",
                "業務を終え、帰宅中の労働者が頭部粉砕で死亡。原因はアパートからの落下物が頭部に直撃したものと見られる。労働者の勤務先である○○○○グループは当事故を労災と判断し、遺族に必要な処置を施した。",
                "労働災害啓発協会",
                "https://github.com/IG-Cultist/WIA-/blob/develop/image/headCrush.png?raw=true"
            ),
            new ItemData(
               "オフィス内での転倒事故",
                4,
                "20XX年XX月XX日",
                "オフィス内にて、物資運搬中に足元不注意で転倒。転倒の際に運搬していたものが他者のPCにぶつかり、業務に多大な影響を及ぼした。事故発生時、オフィスは清掃中であり足元が滑りやすかった。",
                "労働災害啓発協会",
                "https://github.com/IG-Cultist/WIA-/blob/develop/image/office.png?raw=true"
            )
            //),
            //new ItemData(
            //    "労働災害事例報告書",
            //    5,
            //    "20XX年XX月XX日",
            //    "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
            //    "https://引用元.com",
            //    "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/05.png"
            //),
            //new ItemData(
            //    "労働災害事例報告書",
            //    6,
            //    "20XX年XX月XX日",
            //    "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
            //    "https://引用元.com",
            //    "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/06.png"
            //),
            //new ItemData(
            //   "労働災害事例報告書",
            //    7,
            //    "20XX年XX月XX日",
            //    "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
            //    "https://引用元.com",
            //    "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/07.png"
            //),
            //new ItemData(
            //    "労働災害事例報告書",
            //    8,
            //    "20XX年XX月XX日",
            //    "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
            //    "https://引用元.com",
            //    "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/08.png"
            //),
            //new ItemData(
            //   "労働災害事例報告書",
            //    9,
            //    "20XX年XX月XX日",
            //    "本資料では、当プロトコルで体験した労働災害の引用元となる事例を確認することが出来ます。",
            //    "https://引用元.com",
            //    "https://setchi.jp/FancyScrollView/09_LoadTexture/Images/09.png"
            //)
        };

        [SerializeField] ScrollView scrollView = default;

        void Start()
        {
            scrollView.UpdateData(itemData);
        }

        public void BackMenu()
        {
            // シーン遷移
            Initiate.DoneFading();
            //VRかどうかでシーン選別
            if (UnityEngine.XR.XRSettings.isDeviceActive) Initiate.Fade("VR_02_MenuScene", endColor, 2.0f);
            else Initiate.Fade("02_MenuScene", endColor, 2.0f);
        }
    }
}

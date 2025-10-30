using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [Header("�J�ڃt�F�[�h�J���[")]
    [SerializeField]
    Color32 endColor = new Color32(29, 29, 29, 255);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int playerId = MatchingManager.UserID + 1;
        if (GameManager.Result != null)
        {
            Debug.Log("���U���g���擾�ł��܂�");
        }
        else
        {
            Debug.LogError("���U���g���擾�ł��܂���I�I");
        }

            Debug.Log("�v���C���["+playerId+"�̃��U���g�F"+GameManager.Result.TotalScore.ToString());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartTitle()
    {
        // �V�[���J��
        Initiate.DoneFading();
        Initiate.Fade("01_TitleScene", endColor, 1.0f);
    }
}

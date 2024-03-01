using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

//������������Ľű���UI����Ҳд������
public class MainFunction : MonoBehaviour
{
    [HideInInspector]
    public MainFunction Ins;
    //��Ը��Ƶ����GameObject
    [Header("��Ƶ����")]
    public GameObject videoPlayer;
    private VideoPlayer videoPlayerComponent;

    //��Ƶ����GameObject
    [Header("��Ƶ����")]
    public AudioSource audioSource;

    //�������涯������GameObject
    [Header("���涯��")]
    public Animator animator;

    [Header("��������")]
    public GameObject nameGO;

    [Header("�����ٻ���Ч")]
    public AudioClip audioClip3Star;

    [Header("�����ٻ���Ч")]
    public AudioClip audioClip4Star;

    [Header("�����ٻ���Ч")]
    public AudioClip audioClip5Star;

    [Header("���������ٻ���Ƶ")]
    public VideoClip videoClip3Star;

    [Header("���������ٻ���Ƶ")]
    public VideoClip videoClip4Star;

    [Header("���������ٻ���Ƶ")]
    public VideoClip videoClip5Star;

    [Header("ʮ��4���ٻ���Ƶ")]
    public VideoClip videoClip4StarByTenTimes;

    [Header("ʮ��5���ٻ���Ƶ")]
    public VideoClip videoClip5StarByTenTimes;

    [Header("Ĭ��Ԫ��ͼ��")]
    public Sprite defaultElement;

    [Header("Ĭ������")]
    public Sprite defaultCharacter;

    [Header("����UI����GameObject")]
    public GameObject loadingUI;

    [Header("���ؽ�����")]
    public Image loadingProcessBar;

    #region ʮ����ر���
    //�Ƿ�Ϊ10��
    bool isTenTimes = false;

    StudentInfo[] rollStudentInfos = new StudentInfo[10];

    WishAnimationInfo.StarType[] starTypes = new WishAnimationInfo.StarType[10];

    [Header("ʮ��չʾUI")]
    public ShowResultScript[] showResultScripts = new ShowResultScript[10];
    #endregion
    //��¼�鿨��Ϣ�Ķ���
    private Queue<WishAnimationInfo> wishAnimationInfos = new Queue<WishAnimationInfo>();

    //���������Զ����Sprite
    private Dictionary<string, Sprite> customSprites = new Dictionary<string, Sprite>();
    private List<string> spritePaths = new List<string>();
    void Awake()
    {
        Ins = this;
    }

    void Start()
    {
        if (videoPlayer)
        {
            videoPlayerComponent = videoPlayer.GetComponent<VideoPlayer>();
            videoPlayerComponent.loopPointReached += OnVideoEnd;
        }

        //�����Զ��忨��ͼƬ,ע�ʹ�����ֱ�Ӽ��أ������õ���Э�̼��أ����Ը�����Ҫѡ��

        //bool isLoadSuccess = LoadSpriteFromStreamingAssets("/Pictures/KaChi.png", out spriteTemp);
        StartCoroutine(LoadSpriteCoroutine("/Pictures/KaChi.png", (spriteTemp) =>
        {
            GameObject gameObjectTemp = FindGameObject("Image_Pool");
            if (/*isLoadSuccess &&*/ gameObjectTemp)
            {
                gameObjectTemp.GetComponent<Image>().sprite = spriteTemp;
            }
        }));


        //����ѧ������
        string listPath = Application.streamingAssetsPath + "/ListOfStudent.json";
        string jsonString = File.ReadAllText(listPath);
        StudentInfo[] studentInfos = Newtonsoft.Json.JsonConvert.DeserializeObject<StudentInfo[]>(jsonString);
        CallTheRoll.studentInfos.AddRange(studentInfos);

        for (int i = 0; i < CallTheRoll.studentInfos.Count; i++)
        {
            string elementPathTemp = CallTheRoll.studentInfos[i].elementPath;
            string characterPathTemp = CallTheRoll.studentInfos[i].characterPath;
            if (!spritePaths.Contains(elementPathTemp))
            {
                spritePaths.Add(elementPathTemp);
            }
            if (!spritePaths.Contains(characterPathTemp))
            {
                spritePaths.Add(characterPathTemp);
            }
        }

        StartCoroutine(LoadSpriteCoroutine());
    }

    /// <summary>
    /// ��Streaming��ȡSprite
    /// </summary>
    /// <param name="Path">���StreamingAssets��·��</param>
    /// <returns>Sprite</returns>
    bool LoadSpriteFromStreamingAssets(string Path, out Sprite sprite)
    {
        string path = Application.streamingAssetsPath + Path;
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        if (sprite != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// ͨ��Э�̶�ȡSprite
    /// </summary>
    /// <param name="path"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    private IEnumerator LoadSpriteCoroutine(string path, System.Action<Sprite> onComplete)
    {
        string fullPath = Application.streamingAssetsPath + path;
        using (var request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(fullPath))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError ||
                request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error loading sprite from {path}: {request.error}");
                onComplete?.Invoke(null);
            }
            else
            {
                var texture = DownloadHandlerTexture.GetContent(request);
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                onComplete?.Invoke(sprite);
            }
        }
    }

    /// <summary>
    /// ͨ��Э�̶�ȡSprite
    /// </summary>
    /// <param name="path"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    private IEnumerator LoadSpriteCoroutine()
    {
        for (int i = 0; i < spritePaths.Count; i++)
        {
            if (!customSprites.ContainsKey(spritePaths[i]))
            {
                Sprite spriteTemp;
                bool isSuccess = LoadSpriteFromStreamingAssets(spritePaths[i], out spriteTemp);
                if (isSuccess)
                {
                    customSprites.Add(spritePaths[i], spriteTemp);
                }
            }
            loadingProcessBar.fillAmount = (float)(i + 1) / (float)spritePaths.Count;
            yield return 0;
        }
        //���ؼ���UI
        loadingUI.SetActive(false);

        //ѧ����Ϣ������ɺ��л�����UI
        UIFunction.Ins.UIStateMachine.ChangeState(UIFunction.Ins.mainUI);
    }


    /// <summary>
    /// �����ҵ�δ�����GameObject
    /// </summary>
    /// <param name="str">Name</param>
    /// <returns></returns>
    public GameObject FindGameObject(string str)
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject item in all)
        {
            if (item.gameObject.name == str)
            {
                return item;
            }
        }
        return null;
    }

    //����
    public void OnSingleWish()
    {
        isTenTimes = false;
        UIFunction.Ins.UIStateMachine.ChangeState(UIFunction.Ins.videoUI);
        StudentInfo studentInfoTemp;
        bool isRoll = CallTheRoll.RollStudent(out studentInfoTemp);
        WishAnimationInfo.StarType starType = WishAnimationInfo.StarType.Star_3;
        float randomFloat = Random.Range(0f, 1f);

        //�������
        if (randomFloat < 0.006)//0.006�ٷ����ʲ��㱣�׻���
        {
            starType = WishAnimationInfo.StarType.Star_5;
            videoPlayerComponent.clip = videoClip5Star;
        }
        else if (randomFloat < 0.057)//0.057�ٷ����ʲ��㱣�׻���
        {
            starType = WishAnimationInfo.StarType.Star_4;
            videoPlayerComponent.clip = videoClip4Star;
        }
        else
        {
            starType = WishAnimationInfo.StarType.Star_3;
            videoPlayerComponent.clip = videoClip3Star;
        }
        WishAnimationInfo wishAnimationInfo = new WishAnimationInfo(starType, studentInfoTemp);
        wishAnimationInfos.Enqueue(wishAnimationInfo);
        videoPlayerComponent.Play();
    }

    /// <summary>
    /// ʮ������
    /// </summary>
    public void OnTenTimesWish()
    {
        bool isRoll = CallTheRoll.RollStudents(10, out rollStudentInfos);
        if (!isRoll) return;
        isTenTimes = true;
        UIFunction.Ins.UIStateMachine.ChangeState(UIFunction.Ins.videoUI);
        starTypes = new WishAnimationInfo.StarType[10];

        //�Ƿ����4�ǻ���5��
        bool isCotain4Star = false;
        bool isCotain5Star = false;
        for (int i = 0; i < 10; i++)
        {
            float randomFloat = Random.Range(0f, 1f);

            //�������
            if (randomFloat < 0.016)//���ǣ�0.006�ٷ����ʲ��㱣�׻��ƣ�0.016�Ǻ����׵��ۺϸ���
            {
                starTypes[i] = WishAnimationInfo.StarType.Star_5;
                isCotain5Star = true;
            }
            else if (randomFloat < 0.146)//����0.051�ٷ����ʲ��㱣�׻��ƣ������׸�����0.13����Ҫ����ǰ�����Ǹ���
            {
                starTypes[i] = WishAnimationInfo.StarType.Star_4;
                isCotain4Star = true;
            }
            else
            {
                starTypes[i] = WishAnimationInfo.StarType.Star_3;
            }
        }
        if (isCotain5Star)
        {
            videoPlayerComponent.clip = videoClip5StarByTenTimes;
        }
        else if (isCotain4Star)
        {
            videoPlayerComponent.clip = videoClip4StarByTenTimes;
        }
        //û������4�ǵĻ��ֶ�����һ��4��
        else if (!isCotain4Star)
        {
            starTypes[Random.Range(0, 10)] = WishAnimationInfo.StarType.Star_4;
            videoPlayerComponent.clip = videoClip4StarByTenTimes;
        }

        for (int i = 0; i < 10; i++)
        {
            WishAnimationInfo wishAnimationInfo = new WishAnimationInfo(starTypes[i], rollStudentInfos[i]);
            wishAnimationInfos.Enqueue(wishAnimationInfo);
        }

        videoPlayerComponent.Play();
    }

    public void ExitAPP()
    {
        Application.Quit();
    }
    //��������
    public void NextPage()
    {
        if (UIFunction.Ins.UIStateMachine.currentState == UIFunction.Ins.videoUI)
        {
            videoPlayerComponent.Stop();
            if (wishAnimationInfos.Count > 0)
            {
                PlayWishAnimation(wishAnimationInfos.Dequeue());
            }
        }
        else if (UIFunction.Ins.UIStateMachine.currentState == UIFunction.Ins.animationUI)
        {
            if (wishAnimationInfos.Count > 0)
            {
                PlayWishAnimation(wishAnimationInfos.Dequeue());
            }
            else
            {
                if (isTenTimes)
                {
                    UIFunction.Ins.UIStateMachine.ChangeState(UIFunction.Ins.tenTimesUI);
                    for (int i = 0; i < 10; i++)
                    {
                        showResultScripts[i].SetName(starTypes[i], rollStudentInfos[i]);
                    }
                }
                else
                {
                    audioSource.Stop();
                    UIFunction.Ins.UIStateMachine.ChangeState(UIFunction.Ins.mainUI);
                }
            }
        }
        else if (UIFunction.Ins.UIStateMachine.currentState == UIFunction.Ins.tenTimesUI)
        {
            audioSource.Stop();
            UIFunction.Ins.UIStateMachine.ChangeState(UIFunction.Ins.mainUI);
        }
    }

    //�鿨������Ƶ��ɵĻص�����
    private void OnVideoEnd(VideoPlayer vp)
    {
        if (wishAnimationInfos.Count > 0)
        {
            PlayWishAnimation(wishAnimationInfos.Dequeue());
        }
    }

    //�����ٻ�����
    private void PlayWishAnimation(WishAnimationInfo info)
    {
        nameGO.GetComponent<TextMeshProUGUI>().SetText(info.studentInfo.name);
        UIFunction.Ins.UIStateMachine.ChangeState(UIFunction.Ins.animationUI);

        //Ԫ��ͼ��
        GameObject gameObjectTemp = FindGameObject("Image_Element");
        if (customSprites.ContainsKey(info.studentInfo.elementPath) && gameObjectTemp)
        {
            gameObjectTemp.GetComponent<Image>().sprite = customSprites[info.studentInfo.elementPath];
        }
        else
        {
            gameObjectTemp.GetComponent<Image>().sprite = defaultElement;
        }

        //����
        gameObjectTemp = FindGameObject("Image_Character");
        if (customSprites.ContainsKey(info.studentInfo.characterPath) && gameObjectTemp)
        {
            gameObjectTemp.GetComponent<Image>().sprite = customSprites[info.studentInfo.characterPath];
        }
        else
        {
            gameObjectTemp.GetComponent<Image>().sprite = defaultCharacter;
        }

        switch (info.type)
        {
            case WishAnimationInfo.StarType.Star_3:

                animator.Play("3StarAnimation", 0, 0f);
                audioSource.clip = audioClip3Star;
                audioSource.Play();
                break;
            case WishAnimationInfo.StarType.Star_4:
                animator.Play("4StarAnimation", 0, 0f);
                audioSource.clip = audioClip4Star;
                audioSource.Play();
                break;
            case WishAnimationInfo.StarType.Star_5:
                animator.Play("5StarAnimation", 0, 0f);
                audioSource.clip = audioClip5Star;
                audioSource.Play();
                break;
            default:
                break;
        }

    }
}

//����鿨��Ϣ�Ľ���
public class WishAnimationInfo
{
    public WishAnimationInfo(StarType inStarType, StudentInfo inInfo)
    {
        type = inStarType;
        studentInfo = inInfo;
    }
    public enum StarType
    {
        Star_3 = 0,
        Star_4 = 1,
        Star_5 = 2
    }
    public StarType type;
    public StudentInfo studentInfo;
}

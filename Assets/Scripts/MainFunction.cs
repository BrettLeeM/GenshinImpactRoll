using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

//控制整个程序的脚本，UI功能也写里面了
public class MainFunction : MonoBehaviour
{
    [HideInInspector]
    public MainFunction Ins;
    //祈愿视频播放GameObject
    [Header("视频播放")]
    public GameObject videoPlayer;
    private VideoPlayer videoPlayerComponent;

    //音频播放GameObject
    [Header("音频播放")]
    public AudioSource audioSource;

    //人物立绘动画播放GameObject
    [Header("立绘动画")]
    public Animator animator;

    [Header("人物名字")]
    public GameObject nameGO;

    [Header("三星召唤音效")]
    public AudioClip audioClip3Star;

    [Header("四星召唤音效")]
    public AudioClip audioClip4Star;

    [Header("五星召唤音效")]
    public AudioClip audioClip5Star;

    [Header("单抽三星召唤视频")]
    public VideoClip videoClip3Star;

    [Header("单抽四星召唤视频")]
    public VideoClip videoClip4Star;

    [Header("单抽五星召唤视频")]
    public VideoClip videoClip5Star;

    [Header("十连4星召唤视频")]
    public VideoClip videoClip4StarByTenTimes;

    [Header("十连5星召唤视频")]
    public VideoClip videoClip5StarByTenTimes;

    [Header("默认元素图标")]
    public Sprite defaultElement;

    [Header("默认立绘")]
    public Sprite defaultCharacter;

    [Header("加载UI界面GameObject")]
    public GameObject loadingUI;

    [Header("加载进度条")]
    public Image loadingProcessBar;

    #region 十连相关变量
    //是否为10连
    bool isTenTimes = false;

    StudentInfo[] rollStudentInfos = new StudentInfo[10];

    WishAnimationInfo.StarType[] starTypes = new WishAnimationInfo.StarType[10];

    [Header("十连展示UI")]
    public ShowResultScript[] showResultScripts = new ShowResultScript[10];
    #endregion
    //记录抽卡信息的队列
    private Queue<WishAnimationInfo> wishAnimationInfos = new Queue<WishAnimationInfo>();

    //用来储存自定义的Sprite
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

        //加载自定义卡池图片,注释代码是直接加载，现在用的是协程加载，可以根据需要选择

        //bool isLoadSuccess = LoadSpriteFromStreamingAssets("/Pictures/KaChi.png", out spriteTemp);
        StartCoroutine(LoadSpriteCoroutine("/Pictures/KaChi.png", (spriteTemp) =>
        {
            GameObject gameObjectTemp = FindGameObject("Image_Pool");
            if (/*isLoadSuccess &&*/ gameObjectTemp)
            {
                gameObjectTemp.GetComponent<Image>().sprite = spriteTemp;
            }
        }));


        //加载学生名单
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
    /// 从Streaming读取Sprite
    /// </summary>
    /// <param name="Path">相对StreamingAssets的路径</param>
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
    /// 通过协程读取Sprite
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
    /// 通过协程读取Sprite
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
        //隐藏加载UI
        loadingUI.SetActive(false);

        //学生信息加载完成后切换到主UI
        UIFunction.Ins.UIStateMachine.ChangeState(UIFunction.Ins.mainUI);
    }


    /// <summary>
    /// 可以找到未激活的GameObject
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

    //单抽
    public void OnSingleWish()
    {
        isTenTimes = false;
        UIFunction.Ins.UIStateMachine.ChangeState(UIFunction.Ins.videoUI);
        StudentInfo studentInfoTemp;
        bool isRoll = CallTheRoll.RollStudent(out studentInfoTemp);
        WishAnimationInfo.StarType starType = WishAnimationInfo.StarType.Star_3;
        float randomFloat = Random.Range(0f, 1f);

        //单抽概率
        if (randomFloat < 0.006)//0.006官方概率不算保底机制
        {
            starType = WishAnimationInfo.StarType.Star_5;
            videoPlayerComponent.clip = videoClip5Star;
        }
        else if (randomFloat < 0.057)//0.057官方概率不算保底机制
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
    /// 十连代码
    /// </summary>
    public void OnTenTimesWish()
    {
        bool isRoll = CallTheRoll.RollStudents(10, out rollStudentInfos);
        if (!isRoll) return;
        isTenTimes = true;
        UIFunction.Ins.UIStateMachine.ChangeState(UIFunction.Ins.videoUI);
        starTypes = new WishAnimationInfo.StarType[10];

        //是否包含4星或者5星
        bool isCotain4Star = false;
        bool isCotain5Star = false;
        for (int i = 0; i < 10; i++)
        {
            float randomFloat = Random.Range(0f, 1f);

            //单抽概率
            if (randomFloat < 0.016)//五星，0.006官方概率不算保底机制，0.016是含保底的综合概率
            {
                starTypes[i] = WishAnimationInfo.StarType.Star_5;
                isCotain5Star = true;
            }
            else if (randomFloat < 0.146)//四星0.051官方概率不算保底机制，含保底概率是0.13，需要加上前面五星概率
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
        //没有生成4星的话手动设置一个4星
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
    //跳过功能
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

    //抽卡动画视频完成的回调函数
    private void OnVideoEnd(VideoPlayer vp)
    {
        if (wishAnimationInfos.Count > 0)
        {
            PlayWishAnimation(wishAnimationInfos.Dequeue());
        }
    }

    //播放召唤动画
    private void PlayWishAnimation(WishAnimationInfo info)
    {
        nameGO.GetComponent<TextMeshProUGUI>().SetText(info.studentInfo.name);
        UIFunction.Ins.UIStateMachine.ChangeState(UIFunction.Ins.animationUI);

        //元素图标
        GameObject gameObjectTemp = FindGameObject("Image_Element");
        if (customSprites.ContainsKey(info.studentInfo.elementPath) && gameObjectTemp)
        {
            gameObjectTemp.GetComponent<Image>().sprite = customSprites[info.studentInfo.elementPath];
        }
        else
        {
            gameObjectTemp.GetComponent<Image>().sprite = defaultElement;
        }

        //立绘
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

//储存抽卡信息的界面
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

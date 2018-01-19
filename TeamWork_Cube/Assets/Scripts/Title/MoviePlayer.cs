using UnityEngine;
using UnityEngine.Video;

public class MoviePlayer : MonoBehaviour
{    
    public static VideoPlayer mPlayer;
    
    //public VideoClip AndroidRotation;

    VideoClip MouseLeft; //左ドラック操作
    VideoClip MouseRight; //右ドラック操作
    VideoClip MouseWheel; //マウスホイール操作
    VideoClip Bomb; //ボム
    VideoClip Cross; //十字
    VideoClip AllDelete; //色全消し

    GameObject panel;

    // Use this for initialization
    void Start ()
    {
        mPlayer = GetComponent<VideoPlayer>();

        MouseLeft = Resources.Load("Video/SlideVideo") as VideoClip;
        if (Application.platform == RuntimePlatform.Android)
        {
            MouseRight = Resources.Load("Video/Rotation_android") as VideoClip;
        }
        else
        {
            MouseRight = Resources.Load("Video/RotationVideo") as VideoClip;
        }

        MouseWheel = Resources.Load("Video/CameraVideo") as VideoClip;
        Bomb = Resources.Load("Video/BombVideo") as VideoClip;
        Cross = Resources.Load("Video/CrossVideo") as VideoClip;
        AllDelete = Resources.Load("Video/DeleteVideo") as VideoClip;

    }

    // Update is called once per frame
    void Update ()
    {
        VideoChange();
        
        if(mPlayer.isPlaying == true)
        {
            TutorialImageChange.loadTextFlag = true;
        }
        else
        {
            TutorialImageChange.loadTextFlag = false;
        }
    }

    private void VideoChange()
    {
        int num = TutorialImageChange.videoNum;
        switch (num)
        {
            case 1:
                mPlayer.clip = MouseLeft;
                break;
            case 2:
                mPlayer.clip = MouseRight;
                break;
            case 3:
                mPlayer.clip = MouseWheel;
                break;
            case 4:
                mPlayer.clip = Bomb;
                break;
            case 5:
                mPlayer.clip = AllDelete;
                break;
            case 6:
                mPlayer.clip = Cross;
                break;
        }
        mPlayer.Prepare();
        mPlayer.Play();
    }
}

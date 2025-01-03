using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Protocol;
using System.Diagnostics;

[ExecuteAlways]
public class DungeonFinishScore : MonoBehaviour {

    private const string kScoreRootName = "ScoreRoot";

    public float mDelayTime    = 2.0f;

    public Sprite[] mScoreList = new Sprite[0];
    public DungeonScore mScore = DungeonScore.A;

    public Vector3[] mTwoPos = new Vector3[2];
    public Vector3[] mThreePos = new Vector3[3];

    private DungeonScore mRealScore = DungeonScore.A;

    public GameObject mRoot;

    public void SetScore(DungeonScore score)
    {
        if (score != mRealScore)
        {
            mScore = score;
            mRealScore = score;
            _createScoreRoot();
        }
    }


    private Sprite[] _getSprite()
    {
        var strArray = mRealScore.ToString().ToUpper().ToCharArray();

        var scoreLen = mScoreList.Length;
        var strArrayLen = strArray.Length;

        var spriteList = new Sprite[strArrayLen];

        for (int i = 0; i < strArrayLen; ++i)
        {
            var idx = strArray[i] - 'A';

            // get the 'S'
            if (idx >= scoreLen - 1)
            {
                idx = scoreLen - 1;
            }

            spriteList[i] = mScoreList[idx];
        }

        return spriteList;
    }

    private Vector3[] _getPosition(int len)
    {
        if (len == 2)
        {
            return mTwoPos;
        }
        else if (len == 3)
        {
            return mThreePos;
        }

        return null;
    }

    private void _createScoreRoot()
    {
        if (mRoot == null)
        {
            mRoot = gameObject;
        }

        var root = Utility.FindGameObject(mRoot, kScoreRootName, false);
        if (root != null)
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(root);
#else
            GameObject.Destroy(root);
#endif
            root = null;
        }

        root = new GameObject(kScoreRootName, typeof(RectTransform));
        Utility.AttachTo(root, mRoot);

        var spriteList = _getSprite();
        var spriteLen = spriteList.Length;
        var posList = _getPosition(spriteLen);

        for (int i = 0; i < spriteLen; ++i)
        {
            var score = new GameObject("score", typeof(Image));
            Utility.AttachTo(score, root);
            var imageCom = score.GetComponent<Image>();
            imageCom.sprite = spriteList[i];
            imageCom.SetNativeSize();

            if (posList != null)
            {
                var posRect = score.GetComponent<RectTransform>();
                posRect.localPosition = posList[i];
            }
        }
    }

	void Start ()
    {
    }
	

	void Update () {
#if UNITY_EDITOR
        SetScore(mScore);
#endif
    }
}

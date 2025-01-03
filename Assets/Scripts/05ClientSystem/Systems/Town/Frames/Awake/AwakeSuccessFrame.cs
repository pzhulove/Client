using UnityEngine.UI;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    class AwakeSuccessFrame : ClientFrame
    {

        [UIControl("actorpos", typeof(GeAvatarRendererEx))]
        GeAvatarRendererEx avatarRenderer;

        protected override void _OnOpenFrame()
        {
            InitInterface();
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        public override string GetPrefabPath()
        {
            return "UI/Prefabs/AwakeSuccessFrame";
        }

        void ClearData()
        {
        }

        [UIEventHandle("btClose")]
        void OnClose()
        {
            frameMgr.CloseFrame(this);
        }

        void InitInterface()
        {
            var JobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (JobData == null)
            {
                return;
            }

            if (JobData.JobPortrayal != "" && JobData.JobPortrayal != "-")
            {
                //Sprite Icon = AssetLoader.instance.LoadRes(JobData.JobPortrayal, typeof(Sprite)).obj as Sprite;
                //if (Icon != null)
                //{
                //    jobicon.sprite = Icon;
                //}
                ETCImageLoader.LoadSprite(ref jobicon, JobData.JobPortrayal);
            }

            CreateActor();
        }

        void CreateActor()
        {
            JobTable job = TableManager.instance.GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (job == null)
            {
                Logger.LogErrorFormat("can not find JobTable with id:{0}", PlayerBaseData.GetInstance().JobTableID);
            }
            else
            {
                ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);

                if (res == null)
                {
                    Logger.LogErrorFormat("can not find ResTable with id:{0}", job.Mode);
                }
                else
                {
                    //avatarRenderer.Init("Awakeavatar", 530, 566);
                    avatarRenderer.LoadAvatar(res.ModelPath);

                    avatarRenderer.ChangeAction("Anim_Idle01", 1.0f, true);
                    //atarRenderer.SetCameraOrthoSize(1.24f);
                    //atarRenderer.SetViewDirection(new Vector3(10, 0, 0));
                    //atarRenderer.SetCameraPos(new Vector3(0, 1.25f, -0.5f));

                    PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(avatarRenderer);
                }
            }
        }

        [UIControl("jobicon")]
        protected Image jobicon;

        [UIControl("actorpos")]
        protected RawImage actorpos;
    }
}
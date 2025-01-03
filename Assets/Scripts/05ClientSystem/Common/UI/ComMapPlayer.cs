using UnityEngine;

namespace GameClient
{
    class ComMapPlayer : MonoBehaviour
    {
        BeTownPlayer m_player = null;
        ComMapScene m_scene = null;
        BeFighterMain m_fighterPlayer = null;
        Vector2 m_serverPos = Vector2.zero;

        public bool isValid { get { return m_scene != null && m_player != null; } }
        public ComMapScene scene { get { return m_scene; } }
        public Vector3 ServerPos { get { return m_serverPos; } }

        public void Initialize()
        {
            Clear();
        }

        public void Setup(BeTownPlayer a_player, ComMapScene a_comScene)
        {
            if (a_player != null && a_comScene != null)
            {
                m_player = a_player;
                m_scene = a_comScene;

                gameObject.SetActive(true);
                gameObject.transform.SetParent(m_scene.gameObject.transform, false);
                _UpdatePos();
            }
        }
        public void Setup(BeFighterMain a_player, ComMapScene a_comScene)
        {
            if (a_player != null && a_comScene != null)
            {
                m_fighterPlayer = a_player;
                m_scene = a_comScene;

                gameObject.SetActive(true);
                gameObject.transform.SetParent(m_scene.gameObject.transform, false);
                _UpdatePos();
            }

        }

        public void UpdateJobID(int JobTableID)
        {
            if(m_player != null)
            {
                m_player.SetPlayerJobTableID(JobTableID);
            }
        }

        public void Update()
        {
            _UpdatePos();
        }

        public void Clear()
        {
            m_player = null;
            m_scene = null;
            gameObject.SetActive(false);
            m_serverPos = Vector2.zero;
        }

        void _UpdatePos()
        {
            if (m_player != null && m_scene != null)
            {
                BeBaseActorData actorData = m_player.ActorData;

                if (actorData != null)
                {
                    ActorMoveData moveData = actorData.MoveData;

                    if (moveData != null)
                    {
                        Vector3 pos = moveData.ServerPosition;
                        gameObject.transform.localPosition = new Vector3(pos.x * m_scene.XRate, pos.z * m_scene.ZRate, 0.0f);

                        m_serverPos.x = pos.x;
                        m_serverPos.y = pos.z;

                        //Logger.LogErrorFormat("real pos: ({0},{1}) rate: ({2},{3})", pos.x, pos.z, m_scene.XRate, m_scene.ZRate);
                    }
                }
            }
            else if (m_fighterPlayer != null && m_scene != null)
            {
                BeBaseActorData actorData = m_fighterPlayer.ActorData;

                if (actorData != null)
                {
                    ActorMoveData moveData = actorData.MoveData;

                    if (moveData != null)
                    {
                        Vector3 pos = moveData.ServerPosition;
                        gameObject.transform.localPosition = new Vector3(pos.x * m_scene.XRate, pos.z * m_scene.ZRate, 0.0f);

                        m_serverPos.x = pos.x;
                        m_serverPos.y = pos.z;

                        //Logger.LogErrorFormat("real pos: ({0},{1}) rate: ({2},{3})", pos.x, pos.z, m_scene.XRate, m_scene.ZRate);
                    }
                }
            }
        }
    }
}

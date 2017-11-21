using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 说明：战争迷雾表现逻辑，实现所有FOW逻辑表现接口以及与FOWSystem的对接
/// 
/// @by wsh 2017-05-19
/// </summary>

namespace Battle
{
    public class FOWLogic : Singleton<FOWLogic>
    {
        private MapFOWRender m_mapFOWRender;
        
        // 视野体
        private List<IFOWRevealer> m_revealers = new List<IFOWRevealer>();
        // 渲染器
        private List<FOWRender> m_renders = new List<FOWRender>();

        public override void Init()
        {
            base.Init();

            m_revealers.Clear();
            m_renders.Clear();
            FOWSystem.instance.Startup();

            Transform Trans = GameObject.Find("FOWRenderRoot").transform;
            m_mapFOWRender = new MapFOWRender(Trans);

            Messenger.AddListener<int>(MessageName.MN_CHARACTOR_BORN, AddCharactor);
        }

        public override void Dispose()
        {
            Messenger.RemoveListener<int>(MessageName.MN_CHARACTOR_BORN, AddCharactor);

            for (int i = 0; i < m_revealers.Count; i++)
            {
                IFOWRevealer revealer = m_revealers[i];
                if (revealer != null)
                {
                    revealer.Release();
                }
            }
            m_revealers.Clear();

            for (int i = 0; i < m_renders.Count; i++)
            {
                FOWRender render = m_renders[i];
                if (render != null)
                {
                    render.enabled = false;
                    UnityEngine.Object.Destroy(render.gameObject);
                }
            }
            m_renders.Clear();

            m_mapFOWRender = null;
            FOWSystem.instance.DestroySelf();
        }

        protected void AddCharactor(int charaID)
        {
            if (!FOWCharactorRevealer.Contains(charaID))
            {
                if (FOWCharactorRevealer.CheckIsValid(charaID))
                {
                    FOWCharactorRevealer revealer = FOWCharactorRevealer.Get();
                    revealer.InitInfo(charaID);
                    FOWSystem.AddRevealer(revealer);
                    m_revealers.Add(revealer);
                }
            }
        }

        public FOWRender CreateRender(Transform parent)
        {
            if (parent == null)
            {
                return null;
            }

            FOWRender render = null;
            // TODO：实际项目中，从这里的资源管理类加载预设
            // 为了简单，这里直接从Resource加载
            Object prefabs = Resources.Load("Prefabs/FOWRender");
            if (prefabs != null)
            {
                GameObject mesh = GameObject.Instantiate(prefabs) as GameObject;
                if (mesh != null)
                {
                    mesh.transform.parent = parent;
                    render = mesh.gameObject.AddComponent<FOWRender>();
                }
            }

            if (render != null)
            {
                m_renders.Add(render);
            }
            return render;
        }
        
        private void ActivateRender(FOWRender render, bool active)
        {
            if (render != null)
            {
                render.Activate(active);
            }
        }

        public void Update(int deltaMS)
        {
            // 说明：每个游戏帧更新，这里不做时间限制，实测对游戏帧率优化微乎其微
            UpdateRenders();
            UpdateRevealers(deltaMS);
        }

        protected void UpdateRenders()
        {
            for (int i = 0; i < m_renders.Count; i++)
            {
                ActivateRender(m_renders[i], FOWSystem.instance.enableRender);
            }
        }

        protected void UpdateRevealers(int deltaMS)
        {
            for (int i = m_revealers.Count - 1; i >= 0 ; i--)
            {
                IFOWRevealer revealer = m_revealers[i];
                revealer.Update(deltaMS);
                if (!revealer.IsValid())
                {
                    m_revealers.RemoveAt(i);
                    FOWSystem.RemoveRevealer(revealer);
                    revealer.Release();
                }
            }
        }

        public void AddTempRevealer(Vector3 position, float radius, int leftMS)
        {
            if (leftMS <= 0)
            {
                return;
            }
            
            FOWTempRevealer tmpRevealer = FOWTempRevealer.Get();
            tmpRevealer.InitInfo(position, radius, leftMS);
            FOWSystem.AddRevealer(tmpRevealer);
            m_revealers.Add(tmpRevealer);
        }
    }
}

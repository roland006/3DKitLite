using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Gamekit3D
{
    [DefaultExecutionOrder(32000)]
    public class VictoryDanceCutscene : MonoBehaviour
    {
        [Tooltip("Клип из Gangnam Style.fbx (Humanoid, Create From This Model).")]
        public AnimationClip danceClip;
        [Tooltip("FBX Gangnam Style со скином. На него клип играется, позы копируются на Ellen.")]
        public GameObject danceModel;
        public float startDelay = 0.6f;
        public float cameraDistance = 3.2f;
        public float cameraHeight = 1.4f;
        public float orbitSpeed = 18f;

        struct BoneLink
        {
            public Transform src;
            public Transform dst;
            public Quaternion rest;
        }

        bool m_Skip;
        bool m_Playing;
        bool m_DriveCamera;
        bool m_RetargetReady;
        Vector3 m_DesiredCamPos;
        Quaternion m_DesiredCamRot;
        Transform m_Player;
        Camera m_Camera;
        Behaviour m_Brain;
        CameraSettings m_CameraSettings;
        PlayerController m_PlayerController;
        StartUI m_StartUI;
        Animator m_EllenAnimator;
        Animator m_MixamoAnimator;
        GameObject m_Dancer;
        PlayableGraph m_Graph;
        AnimationClipPlayable m_DancePlayable;
        readonly List<BoneLink> m_Bones = new List<BoneLink>();
        Transform m_SrcHips;
        Vector3 m_EllenStartPos;
        float m_SrcHipsStartY;
        bool m_SavedEllenAnimator;

        static readonly string[][] k_BoneMap =
        {
            new[] { "Hips", "Ellen_Hips" },
            new[] { "Spine", "Ellen_Spine" },
            new[] { "Spine1", "Ellen_Chest" },
            new[] { "Spine2", "Ellen_UpperChest" },
            new[] { "Neck", "Ellen_Neck" },
            new[] { "Head", "Ellen_Head" },
            new[] { "LeftShoulder", "Ellen_Left_Shoulder" },
            new[] { "LeftArm", "Ellen_Left_UpperArm" },
            new[] { "LeftForeArm", "Ellen_Left_Arm" },
            new[] { "LeftHand", "Ellen_Left_Hand" },
            new[] { "RightShoulder", "Ellen_Right_Shoulder" },
            new[] { "RightArm", "Ellen_Right_UpperArm" },
            new[] { "RightForeArm", "Ellen_Right_Arm" },
            new[] { "RightHand", "Ellen_Right_Hand" },
            new[] { "LeftUpLeg", "Ellen_Left_UpperLeg" },
            new[] { "LeftLeg", "Ellen_Left_LowerLeg" },
            new[] { "LeftFoot", "Ellen_Left_Foot" },
            new[] { "LeftToeBase", "Ellen_Left_Toes" },
            new[] { "RightUpLeg", "Ellen_Right_UpperLeg" },
            new[] { "RightLeg", "Ellen_Right_LowerLeg" },
            new[] { "RightFoot", "Ellen_Right_Foot" },
            new[] { "RightToeBase", "Ellen_Right_Toes" }
        };

        void Start()
        {
            GrenadierBehaviour[] grenadiers = FindObjectsOfType<GrenadierBehaviour>(true);
            for (int i = 0; i < grenadiers.Length; i++)
            {
                if (grenadiers[i] == null)
                    continue;
                Damageable damageable = grenadiers[i].GetComponentInChildren<Damageable>(true);
                if (damageable != null)
                    damageable.OnDeath.AddListener(OnGrenadierDeath);
            }
        }

        void Update()
        {
            if (m_Playing && Input.GetKeyDown(KeyCode.Escape))
                m_Skip = true;
        }

        void LateUpdate()
        {
            if (m_Playing && m_DancePlayable.IsValid() && danceClip != null && danceClip.length > 0.01f)
            {
                if (m_DancePlayable.GetTime() >= danceClip.length)
                    m_DancePlayable.SetTime(0.0);
            }

            if (m_RetargetReady)
                ApplyRetarget();

            if (!m_DriveCamera || m_Camera == null)
                return;
            m_Camera.transform.SetPositionAndRotation(m_DesiredCamPos, m_DesiredCamRot);
        }

        void OnDestroy()
        {
            StopDanceGraph();
        }

        void OnGrenadierDeath()
        {
            if (m_Playing)
                return;
            StartCoroutine(PlayRoutine());
        }

        IEnumerator PlayRoutine()
        {
            m_Playing = true;
            m_Skip = false;

            if (startDelay > 0f)
                yield return new WaitForSeconds(startDelay);

            m_Player = FindPlayer();
            m_Camera = Camera.main;
            if (m_Player == null || m_Camera == null)
            {
                m_Playing = false;
                yield break;
            }

            m_PlayerController = m_Player.GetComponent<PlayerController>();
            m_EllenAnimator = m_Player.GetComponent<Animator>();
            m_StartUI = FindObjectOfType<StartUI>();

            PlayerInput input = PlayerInput.Instance != null
                ? PlayerInput.Instance
                : FindObjectOfType<PlayerInput>();
            Damageable playerDamage = m_Player.GetComponent<Damageable>();
            bool wasInvulnerable = playerDamage != null && playerDamage.isInvulnerable;
            bool savedUi = m_StartUI != null && m_StartUI.enabled;
            bool savedPc = m_PlayerController != null && m_PlayerController.enabled;

            if (input != null)
                input.ReleaseControl();
            if (playerDamage != null)
                playerDamage.isInvulnerable = true;
            if (m_StartUI != null)
                m_StartUI.enabled = false;
            if (m_PlayerController != null)
                m_PlayerController.enabled = false;

            TakeOverCamera();
            yield return StartCoroutine(StartDance());

            float angle = Mathf.Atan2(-m_Player.forward.x, -m_Player.forward.z);
            if (float.IsNaN(angle))
                angle = 0f;

            while (!m_Skip)
            {
                Vector3 center = m_Player.position + Vector3.up * 1.15f;
                angle += orbitSpeed * Mathf.Deg2Rad * Time.deltaTime;
                Vector3 pos = new Vector3(
                    center.x + Mathf.Sin(angle) * cameraDistance,
                    center.y + cameraHeight,
                    center.z + Mathf.Cos(angle) * cameraDistance);
                Vector3 toLook = center - pos;
                if (toLook.sqrMagnitude < 0.0001f)
                    toLook = m_Player.forward;
                m_DesiredCamPos = pos;
                m_DesiredCamRot = Quaternion.LookRotation(toLook, Vector3.up);
                m_DriveCamera = true;
                yield return null;
            }

            StopDance();
            ReleaseCamera();

            if (m_PlayerController != null)
                m_PlayerController.enabled = savedPc;
            if (m_StartUI != null)
                m_StartUI.enabled = savedUi;
            if (playerDamage != null)
                playerDamage.isInvulnerable = wasInvulnerable;
            if (input != null)
                input.GainControl();

            m_Playing = false;
            m_DriveCamera = false;
        }

        IEnumerator StartDance()
        {
            if (danceClip == null)
                danceClip = FindGangnamClip();
            if (danceModel == null)
                danceModel = FindGangnamModel();

            if (danceClip == null || danceModel == null)
            {
                Debug.LogWarning("VictoryDanceCutscene: нужен Dance Clip и Dance Model (FBX Gangnam Style со скином, Humanoid, Create From This Model). На Ellen клип Mixamo сам не ляжет — у неё Generic.");
                yield break;
            }

            m_EllenStartPos = m_Player.position;
            m_Dancer = Instantiate(danceModel, m_Player.position, m_Player.rotation);
            m_Dancer.name = "GangnamSource";
            StripAndHide(m_Dancer);

            m_MixamoAnimator = m_Dancer.GetComponentInChildren<Animator>();
            if (m_MixamoAnimator == null)
                m_MixamoAnimator = m_Dancer.AddComponent<Animator>();
            m_MixamoAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            m_MixamoAnimator.applyRootMotion = false;

            if (m_EllenAnimator != null)
            {
                m_SavedEllenAnimator = m_EllenAnimator.enabled;
                m_EllenAnimator.enabled = false;
            }

            yield return null;
            BuildBoneLinks();

            StopDanceGraph();
            m_Graph = PlayableGraph.Create("MixamoGangnam");
            m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(m_Graph, "Animation", m_MixamoAnimator);
            m_DancePlayable = AnimationClipPlayable.Create(m_Graph, danceClip);
            m_DancePlayable.SetDuration(danceClip.length);
            output.SetSourcePlayable(m_DancePlayable);
            m_Graph.Play();

            if (m_SrcHips != null)
                m_SrcHipsStartY = m_SrcHips.position.y;
            m_RetargetReady = m_Bones.Count > 0;
        }

        void StopDance()
        {
            m_RetargetReady = false;
            StopDanceGraph();
            if (m_EllenAnimator != null)
            {
                m_EllenAnimator.enabled = m_SavedEllenAnimator;
                m_EllenAnimator.Play("Locomotion", 0, 0f);
            }
            if (m_Dancer != null)
            {
                Destroy(m_Dancer);
                m_Dancer = null;
            }
            m_MixamoAnimator = null;
            m_Bones.Clear();
            if (m_Player != null)
                m_Player.position = m_EllenStartPos;
        }

        void BuildBoneLinks()
        {
            m_Bones.Clear();
            if (m_Dancer == null || m_Player == null)
                return;

            for (int i = 0; i < k_BoneMap.Length; i++)
            {
                Transform src = FindBone(m_Dancer.transform, k_BoneMap[i][0]);
                Transform dst = FindBone(m_Player, k_BoneMap[i][1]);
                if (src == null || dst == null)
                    continue;
                BoneLink link;
                link.src = src;
                link.dst = dst;
                link.rest = Quaternion.Inverse(src.rotation) * dst.rotation;
                m_Bones.Add(link);
                if (k_BoneMap[i][1] == "Ellen_Hips")
                    m_SrcHips = src;
            }
        }

        void ApplyRetarget()
        {
            for (int i = 0; i < m_Bones.Count; i++)
            {
                BoneLink link = m_Bones[i];
                if (link.src != null && link.dst != null)
                    link.dst.rotation = link.src.rotation * link.rest;
            }

            if (m_SrcHips != null && m_Player != null)
            {
                float jump = m_SrcHips.position.y - m_SrcHipsStartY;
                Vector3 p = m_EllenStartPos;
                p.y += jump;
                m_Player.position = p;
            }
        }

        static Transform FindBone(Transform root, string token)
        {
            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            string exact = token.ToLowerInvariant();
            Transform best = null;
            int bestScore = 1000;
            for (int i = 0; i < all.Length; i++)
            {
                string n = all[i].name.ToLowerInvariant();
                int colon = n.LastIndexOf(':');
                string leaf = colon >= 0 ? n.Substring(colon + 1) : n;
                if (leaf == exact)
                    return all[i];
                if (leaf.EndsWith(exact) && leaf.Length - exact.Length < bestScore)
                {
                    bestScore = leaf.Length - exact.Length;
                    best = all[i];
                }
            }
            return best;
        }

        static void StripAndHide(GameObject go)
        {
            Renderer[] rends = go.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < rends.Length; i++)
                rends[i].enabled = false;
            Camera[] cams = go.GetComponentsInChildren<Camera>(true);
            for (int i = 0; i < cams.Length; i++)
            {
                cams[i].enabled = false;
                if (cams[i].CompareTag("MainCamera"))
                    cams[i].tag = "Untagged";
            }
            AudioListener[] listeners = go.GetComponentsInChildren<AudioListener>(true);
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].enabled = false;
            Light[] lights = go.GetComponentsInChildren<Light>(true);
            for (int i = 0; i < lights.Length; i++)
                lights[i].enabled = false;
            Collider[] cols = go.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < cols.Length; i++)
                cols[i].enabled = false;
        }

        void StopDanceGraph()
        {
            if (m_Graph.IsValid())
                m_Graph.Destroy();
        }

        static AnimationClip FindGangnamClip()
        {
            AnimationClip[] clips = Resources.FindObjectsOfTypeAll<AnimationClip>();
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null && clips[i].name.ToLowerInvariant().Contains("gangnam"))
                    return clips[i];
            }
            return null;
        }

        static GameObject FindGangnamModel()
        {
            GameObject named = GameObject.Find("Gangnam Style");
            if (named != null)
                return named;
            return null;
        }

        void TakeOverCamera()
        {
            m_Brain = FindBrain(m_Camera.transform);
            if (m_Brain != null)
                m_Brain.enabled = false;
            m_CameraSettings = m_Camera.GetComponentInParent<CameraSettings>();
            if (m_CameraSettings == null)
                m_CameraSettings = FindObjectOfType<CameraSettings>();
            if (m_CameraSettings != null)
                m_CameraSettings.enabled = false;
        }

        void ReleaseCamera()
        {
            if (m_CameraSettings != null)
                m_CameraSettings.enabled = true;
            if (m_Brain != null)
                m_Brain.enabled = true;
        }

        static Behaviour FindBrain(Transform from)
        {
            Transform t = from;
            while (t != null)
            {
                Behaviour[] behaviours = t.GetComponents<Behaviour>();
                for (int i = 0; i < behaviours.Length; i++)
                {
                    if (behaviours[i] != null && behaviours[i].GetType().Name == "CinemachineBrain")
                        return behaviours[i];
                }
                t = t.parent;
            }
            return null;
        }

        static Transform FindPlayer()
        {
            PlayerController pc = PlayerController.instance != null
                ? PlayerController.instance
                : FindObjectOfType<PlayerController>();
            if (pc != null)
                return pc.transform;
            GameObject tagged = GameObject.FindGameObjectWithTag("Player");
            return tagged != null ? tagged.transform : null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamekit3D
{
    [DefaultExecutionOrder(32000)]
    public class IntroFlybyCutscene : MonoBehaviour
    {
        public bool playOnStart = true;
        public Transform helicopter;
        public Transform weaponPedestal;
        public float startDelay = 0.35f;
        public float fadeDuration = 0.5f;
        [Range(10f, 15f)]
        public float orbitDegrees = 12f;
        public float nearbyEnemyRadius = 55f;
        public int nearbyEnemyShots = 3;
        public float playerShotDuration = 2.2f;
        public float enemyShotDuration = 2.4f;
        public float bossShotDuration = 3.2f;
        public float helicopterShotDuration = 3.2f;
        public float weaponShotDuration = 2.4f;
        public float returnShotDuration = 1.8f;
        public float nearbyCameraDistance = 2.6f;
        public float bossCameraDistance = 5.2f;
        public float helicopterCameraDistance = 8f;
        public float weaponCameraDistance = 2.8f;

        bool m_Skip;
        Transform m_Player;
        Camera m_Camera;
        Behaviour m_Brain;
        CameraSettings m_CameraSettings;
        CanvasGroup m_Fade;
        bool m_DriveCamera;
        Vector3 m_DesiredCamPos;
        Quaternion m_DesiredCamRot;
        readonly List<EnemyController> m_PausedEnemies = new List<EnemyController>();

        struct Shot
        {
            public Transform look;
            public float distance;
            public float duration;
        }

        void Start()
        {
            if (playOnStart)
                StartCoroutine(PlayRoutine());
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
                m_Skip = true;
        }

        void LateUpdate()
        {
            if (!m_DriveCamera || m_Camera == null)
                return;
            m_Camera.transform.SetPositionAndRotation(m_DesiredCamPos, m_DesiredCamRot);
        }

        IEnumerator PlayRoutine()
        {
            yield return null;
            if (startDelay > 0f)
                yield return new WaitForSeconds(startDelay);

            m_Player = FindPlayer();
            m_Camera = Camera.main;
            if (m_Player == null || m_Camera == null)
                yield break;

            List<Shot> shots = BuildShots();
            if (shots.Count == 0)
                yield break;

            PlayerInput input = PlayerInput.Instance != null
                ? PlayerInput.Instance
                : FindObjectOfType<PlayerInput>();
            Damageable damageable = m_Player.GetComponent<Damageable>();
            bool wasInvulnerable = damageable != null && damageable.isInvulnerable;

            if (input != null)
                input.ReleaseControl();
            if (damageable != null)
                damageable.isInvulnerable = true;

            PauseNearbyEnemies();
            TakeOverCamera();
            CreateFadeOverlay();

            for (int i = 0; i < shots.Count; i++)
            {
                if (m_Skip)
                    break;

                yield return FadeTo(1f);
                if (m_Skip)
                    break;

                yield return HoldShot(shots[i], i);
            }

            yield return FadeTo(1f);
            ReleaseCamera();
            yield return FadeTo(0f);

            DestroyFadeOverlay();
            ResumeEnemies();

            if (damageable != null)
                damageable.isInvulnerable = wasInvulnerable;
            if (input != null)
                input.GainControl();
        }

        List<Shot> BuildShots()
        {
            var shots = new List<Shot>();

            shots.Add(MakeShot(m_Player, nearbyCameraDistance, playerShotDuration));

            Transform weapon = weaponPedestal != null ? weaponPedestal : FindWeaponPedestal();
            if (weapon != null)
                shots.Add(MakeShot(weapon, weaponCameraDistance, weaponShotDuration));

            List<Transform> nearby = FindNearbyEnemies();
            nearby.Sort((a, b) =>
                (a.position - m_Player.position).sqrMagnitude.CompareTo(
                    (b.position - m_Player.position).sqrMagnitude));

            int count = Mathf.Min(nearbyEnemyShots, nearby.Count);
            for (int i = 0; i < count; i++)
                shots.Add(MakeShot(nearby[i], nearbyCameraDistance, enemyShotDuration));

            Transform boss = FindBoss();
            if (boss != null)
                shots.Add(MakeShot(boss, bossCameraDistance, bossShotDuration));

            if (helicopter != null)
                shots.Add(MakeShot(helicopter, helicopterCameraDistance, helicopterShotDuration));

            shots.Add(MakeShot(m_Player, nearbyCameraDistance, returnShotDuration));
            return shots;
        }

        static Shot MakeShot(Transform look, float distance, float duration)
        {
            return new Shot { look = look, distance = distance, duration = duration };
        }

        IEnumerator HoldShot(Shot shot, int index)
        {
            if (shot.look == null || m_Camera == null)
                yield break;

            Vector3 center;
            Vector3 startPos = FindClearCameraPose(shot.look, shot.distance, out center);
            SetCameraLookAt(startPos, center, shot.look);

            yield return FadeTo(0f);
            if (m_Skip)
                yield break;

            Vector3 flat = startPos - center;
            flat.y = 0f;
            if (flat.sqrMagnitude < 0.01f)
                flat = -FlatForward(shot.look);
            float radius = Mathf.Max(1.2f, flat.magnitude);
            float startAngle = Mathf.Atan2(flat.x, flat.z);
            float sweep = Mathf.Clamp(orbitDegrees, 10f, 15f) * Mathf.Deg2Rad;
            if (index % 2 != 0)
                sweep = -sweep;
            float height = startPos.y;

            float t = 0f;
            while (t < shot.duration && !m_Skip)
            {
                t += Time.deltaTime;
                float u = Smooth(Mathf.Clamp01(t / Mathf.Max(0.01f, shot.duration)));
                float angle = startAngle + sweep * u;

                Vector3 pos = new Vector3(
                    center.x + Mathf.Sin(angle) * radius,
                    height,
                    center.z + Mathf.Cos(angle) * radius);

                SetCameraLookAt(pos, center, shot.look);
                yield return null;
            }
        }

        void SetCameraLookAt(Vector3 camPos, Vector3 lookPoint, Transform fallback)
        {
            Vector3 toLook = lookPoint - camPos;
            if (toLook.sqrMagnitude < 0.0001f)
                toLook = fallback.forward;
            m_DesiredCamPos = camPos;
            m_DesiredCamRot = Quaternion.LookRotation(toLook, Vector3.up);
            m_DriveCamera = true;
            if (m_Camera != null)
                m_Camera.transform.SetPositionAndRotation(m_DesiredCamPos, m_DesiredCamRot);
        }

        static float Smooth(float x)
        {
            return x * x * (3f - 2f * x);
        }

        void CreateFadeOverlay()
        {
            GameObject root = new GameObject("IntroFlybyFade");
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32000;
            root.AddComponent<CanvasScaler>();

            m_Fade = root.AddComponent<CanvasGroup>();
            m_Fade.alpha = 0f;
            m_Fade.blocksRaycasts = false;
            m_Fade.interactable = false;

            GameObject panel = new GameObject("Black");
            panel.transform.SetParent(root.transform, false);
            Image image = panel.AddComponent<Image>();
            image.color = Color.black;
            image.raycastTarget = false;

            RectTransform rt = image.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        void DestroyFadeOverlay()
        {
            if (m_Fade != null)
            {
                Destroy(m_Fade.gameObject);
                m_Fade = null;
            }
        }

        IEnumerator FadeTo(float target)
        {
            if (m_Fade == null)
                yield break;

            float start = m_Fade.alpha;
            float duration = Mathf.Max(0.01f, fadeDuration);
            float t = 0f;
            while (t < duration && !m_Skip)
            {
                t += Time.deltaTime;
                m_Fade.alpha = Mathf.Lerp(start, target, Mathf.Clamp01(t / duration));
                yield return null;
            }

            if (!m_Skip)
                m_Fade.alpha = target;
        }

        void TakeOverCamera()
        {
            m_DriveCamera = true;
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
            m_DriveCamera = false;
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

        Vector3 FindClearCameraPose(Transform target, float preferredDistance, out Vector3 lookPoint)
        {
            lookPoint = GetLookPoint(target);
            float radius = GetVisualRadius(target);
            float distance = Mathf.Max(1.4f, preferredDistance, radius * 1.7f + 1.1f);
            const float probe = 0.22f;
            float minClear = 1.15f + radius * 0.25f;

            Vector3[] dirs = BuildCandidateDirections(target);
            Vector3 bestPos = lookPoint - FlatForward(target) * distance + Vector3.up * 0.4f;
            float bestClear = -1f;

            for (int i = 0; i < dirs.Length; i++)
            {
                Vector3 dir = dirs[i];
                float clear = ProbeClearDistance(lookPoint, dir, distance, probe, target);
                if (clear < minClear)
                    continue;

                Vector3 pos = lookPoint + dir * Mathf.Min(clear, distance);
                if (IsBlockingSphere(pos, probe, target))
                    pos = lookPoint + dir * Mathf.Max(minClear, clear - probe - 0.12f);

                if (clear >= distance - 0.05f)
                    return pos;

                if (clear > bestClear)
                {
                    bestClear = clear;
                    bestPos = pos;
                }
            }

            return bestPos;
        }

        static Vector3 FlatForward(Transform target)
        {
            Vector3 fwd = target.forward;
            fwd.y = 0f;
            if (fwd.sqrMagnitude < 0.001f)
                fwd = Vector3.forward;
            return fwd.normalized;
        }

        static Vector3[] BuildCandidateDirections(Transform target)
        {
            Vector3 fwd = FlatForward(target);
            Vector3 right = Vector3.Cross(Vector3.up, fwd).normalized;

            return new[]
            {
                (-fwd + Vector3.up * 0.22f).normalized,
                (-fwd + right * 0.45f + Vector3.up * 0.18f).normalized,
                (-fwd - right * 0.45f + Vector3.up * 0.18f).normalized,
                (right + Vector3.up * 0.2f).normalized,
                (-right + Vector3.up * 0.2f).normalized,
                (fwd * 0.35f + right + Vector3.up * 0.25f).normalized,
                (fwd * 0.35f - right + Vector3.up * 0.25f).normalized,
                (-fwd + Vector3.up * 0.55f).normalized
            };
        }

        float ProbeClearDistance(Vector3 origin, Vector3 dir, float maxDistance, float probe, Transform target)
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                origin, probe, dir, maxDistance, ~0, QueryTriggerInteraction.Ignore);

            float nearest = maxDistance;
            for (int i = 0; i < hits.Length; i++)
            {
                if (ShouldIgnoreCollider(hits[i].collider, target))
                    continue;
                if (hits[i].distance < nearest)
                    nearest = hits[i].distance;
            }

            return Mathf.Max(0f, nearest - probe - 0.08f);
        }

        bool IsBlockingSphere(Vector3 pos, float probe, Transform target)
        {
            Collider[] cols = Physics.OverlapSphere(pos, probe, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < cols.Length; i++)
            {
                if (!ShouldIgnoreCollider(cols[i], target))
                    return true;
            }
            return false;
        }

        bool ShouldIgnoreCollider(Collider col, Transform target)
        {
            if (col == null)
                return true;
            Transform t = col.transform;
            if (t.IsChildOf(target) || target.IsChildOf(t))
                return true;
            if (m_Player != null && (t.IsChildOf(m_Player) || m_Player.IsChildOf(t)))
                return true;
            return false;
        }

        static Vector3 GetLookPoint(Transform target)
        {
            Bounds bounds;
            if (TryGetRenderBounds(target, out bounds))
                return bounds.center;
            return target.position + Vector3.up * 1.1f;
        }

        static float GetVisualRadius(Transform target)
        {
            Bounds bounds;
            if (!TryGetRenderBounds(target, out bounds))
                return 1f;
            return Mathf.Max(0.6f, bounds.extents.magnitude * 0.55f);
        }

        static bool TryGetRenderBounds(Transform target, out Bounds bounds)
        {
            bounds = new Bounds(target.position, Vector3.one);
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            bool hasBounds = false;
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null || renderers[i] is ParticleSystemRenderer)
                    continue;
                if (!hasBounds)
                {
                    bounds = renderers[i].bounds;
                    hasBounds = true;
                }
                else
                    bounds.Encapsulate(renderers[i].bounds);
            }
            return hasBounds;
        }

        void PauseNearbyEnemies()
        {
            m_PausedEnemies.Clear();
            float maxSqr = nearbyEnemyRadius * nearbyEnemyRadius * 4f;
            EnemyController[] enemies = FindObjectsOfType<EnemyController>(true);
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == null || !enemies[i].enabled)
                    continue;
                if ((enemies[i].transform.position - m_Player.position).sqrMagnitude > maxSqr)
                    continue;
                enemies[i].enabled = false;
                m_PausedEnemies.Add(enemies[i]);
            }
        }

        void ResumeEnemies()
        {
            for (int i = 0; i < m_PausedEnemies.Count; i++)
            {
                if (m_PausedEnemies[i] != null)
                    m_PausedEnemies[i].enabled = true;
            }
            m_PausedEnemies.Clear();
        }

        static Transform FindPlayer()
        {
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc != null)
                return pc.transform;
            GameObject tagged = GameObject.FindGameObjectWithTag("Player");
            return tagged != null ? tagged.transform : null;
        }

        List<Transform> FindNearbyEnemies()
        {
            var result = new List<Transform>();
            float maxSqr = nearbyEnemyRadius * nearbyEnemyRadius;

            ChomperBehavior[] chompers = FindObjectsOfType<ChomperBehavior>(true);
            for (int i = 0; i < chompers.Length; i++)
                AddIfNearby(result, chompers[i].transform, maxSqr);

            SpitterBehaviour[] spitters = FindObjectsOfType<SpitterBehaviour>(true);
            for (int i = 0; i < spitters.Length; i++)
                AddIfNearby(result, spitters[i].transform, maxSqr);

            return result;
        }

        void AddIfNearby(List<Transform> list, Transform t, float maxSqr)
        {
            if (t == null)
                return;
            if ((t.position - m_Player.position).sqrMagnitude <= maxSqr)
                list.Add(t);
        }

        static Transform FindBoss()
        {
            GrenadierBehaviour grenadier = FindObjectOfType<GrenadierBehaviour>(true);
            return grenadier != null ? grenadier.transform : null;
        }

        static Transform FindWeaponPedestal()
        {
            GameObject named = GameObject.Find("WeaponPedestal");
            if (named != null)
                return named.transform;

            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                Transform found = FindNamed(roots[i].transform, "weaponpedestal", "weaponpedastal");
                if (found != null)
                    return found;
            }
            return null;
        }

        static Transform FindNamed(Transform root, string keyA, string keyB)
        {
            string n = root.name.ToLowerInvariant();
            if (n == keyA || n == keyB || n.Contains(keyA) || n.Contains(keyB))
                return root;
            for (int i = 0; i < root.childCount; i++)
            {
                Transform found = FindNamed(root.GetChild(i), keyA, keyB);
                if (found != null)
                    return found;
            }
            return null;
        }
    }
}
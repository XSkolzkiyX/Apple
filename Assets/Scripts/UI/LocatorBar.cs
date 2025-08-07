using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LocatorBar : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform compassBar;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private float maxAngle = 90f;
    [SerializeField] private float barWidth = 500f;

    private Camera mainCamera;

    [System.Serializable]
    public class CompassTarget
    {
        public Transform target;
        public Image iconInstance;
    }

    [SerializeField] private List<CompassTarget> targets = new List<CompassTarget>();

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse2))
        {
            if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit))
            {
                GameObject target = new GameObject("Locator Point #" + (targets.Count + 1));
                target.transform.position = hit.point;
                AddTarget(target.transform);
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (var target in targets)
        {
            Vector3 flatForward = new Vector3(player.forward.x, 0f, player.forward.z).normalized;
            Vector3 toTarget = (target.target.position - player.position);
            Vector3 flatToTarget = new Vector3(toTarget.x, 0f, toTarget.z).normalized;

            float angle = Vector3.SignedAngle(flatForward, flatToTarget, Vector3.up);

            if (Mathf.Abs(angle) > maxAngle)
            {
                target.iconInstance.gameObject.SetActive(false);
                continue;
            }

            target.iconInstance.gameObject.SetActive(true);
            float normalized = angle / maxAngle;
            float xPos = normalized * (barWidth / 2f);

            target.iconInstance.rectTransform.anchoredPosition = new Vector2(xPos, 0);
        }
    }

    public void AddTarget(Transform target)
    {
        var icon = Instantiate(iconPrefab, compassBar).GetComponent<Image>();
        targets.Add(new CompassTarget { target = target, iconInstance = icon });
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Background_Scroll : MonoBehaviour
{
    /*
    public Transform cameraTransform;
    public float backgroundWidth;
    [Range(-1f, 1f)] public float parallaxFactorX;
    public bool lockYToCamera = true;
    [Range(0f, 100f)] public float yFollowSmoothness; // 추가: 부드럽게 따라오는 속도

    private Vector3 lastCameraPos;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraPos = cameraTransform.position;
    }

    void Update()
    {
        Vector3 delta = cameraTransform.position - lastCameraPos;

        // X축 패럴랙스
        transform.position += new Vector3(delta.x * parallaxFactorX, 0f, 0f);

        // Y축 부드럽게 따라오기
        if (lockYToCamera)
        {
            float smoothY = Mathf.Lerp(
                transform.position.y,
                cameraTransform.position.y,
                Time.deltaTime * yFollowSmoothness
            );

            transform.position = new Vector3(
                transform.position.x,
                smoothY,
                transform.position.z
            );
        }

        lastCameraPos = cameraTransform.position;

        // X축 무한 반복
        if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= backgroundWidth)
        {
            float offset = (cameraTransform.position.x > transform.position.x)
                ? backgroundWidth * 2f
                : -backgroundWidth * 2f;

            transform.position += Vector3.right * offset;
        }
    }
    */
    /*
    [SerializeField] private Player_Move player_Move;

    [Header("---Setting---")]
    [SerializeField] private List<Background> backgrounds;

    [System.Serializable]
    public struct Background
    {
        public float speed;
        public Material materials;
    }
    private void Update()
    {
        if(player_Move.moveVector.sqrMagnitude > 0)
        {
            for (int i = 0; i < backgrounds.Count; i++)
            {
                Vector2 dir = new Vector2(player_Move.moveVector.x, 0);
                backgrounds[i].materials.SetTextureOffset("_MainTex", backgrounds[i].speed * Time.time * dir);
            }
        }
    }
    */
    /*
    [SerializeField] private Material[] materials;
    [SerializeField] private float[] layer_MoveSpeed;


    [SerializeField][Range(0.01f, 1.0f)] private float parallax_Speed;

    private void Awake()
    {
        int backgroundCount = transform.childCount;
        GameObject[]  backgrounds = new GameObject[backgroundCount];

        materials = new Material[backgroundCount];
        layer_MoveSpeed = new float[backgroundCount];

        for(int i = 0; i < backgroundCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            materials[i] = backgrounds[i].GetComponent<Renderer>().material;
        }
    }
    */

    [SerializeField] Player_Move playerMove;

    [SerializeField] private List<MatData> matData;

    private void Update()
    {
        Scroll();
    }

    void Scroll()
    {
        if (playerMove.moveVector.sqrMagnitude > 0)
        {
            for (int i = 0; i < matData.Count; i++)
            {
                if(playerMove.moveVector.x < 0)
                {
                    matData[i].value -= matData[i].moveSpeed * Time.deltaTime;
                }
                else
                {
                    matData[i].value += matData[i].moveSpeed * Time.deltaTime;
                }

                matData[i].mat.SetTextureOffset("_MainTex", new Vector2(matData[i].value, 0));
            }
        }
    }

    [System.Serializable]
    public class MatData
    {
        public Material mat;
        public float moveSpeed;
        public float value;
    }
}

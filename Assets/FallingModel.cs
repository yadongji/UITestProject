using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FallingModel : MonoBehaviour
{
    [Header("�������")]
    public float h = 2.0f; // Բ���峤��
    public float gravityA = 10f; // A���������ٶ�
    public float gravityB = 10f; // B���������ٶ�
    public float cylinderARadius = 0.2f; // A�İ뾶
    public float cylinderBRadius = 0.5f; // B�İ뾶

    [Header("�ο�����")]
    public GameObject cylinderA; // ϸԲ����A
    public GameObject cylinderB; // ��Բ����B

    // ����״̬
    private bool isFallingA = true;
    private bool isFallingB = false;
    private bool collisionOccured = false;
    private float collisionStartTime = 0f;
    private float collisionDuration = 0f;
    public Button activeBtn;
    // �������
    private Rigidbody rbA;
    private Rigidbody rbB;

    private GUIStyle fontStyle;
    public GameObject refreshPlane;
    // ��ʼλ��
    private Vector3 initialPosA;
    private Vector3 initialPosB;
    void Start()
    {
        // �����ʼλ��
        initialPosB = cylinderB.transform.position;
        initialPosA = new Vector3(0, initialPosB.y + 2*h, 0);
        fontStyle = new GUIStyle();
        fontStyle.fontSize = 60;
        // ��ʼ������λ�úʹ�С
        InitializeCylinders();

        // ��ȡ����Ӹ������
        SetupRigidbodies();
        activeBtn.onClick.AddListener(()=> { activeBtn.gameObject.SetActive(false);
            Time.timeScale = 0f; refreshPlane.SetActive(true);
            ResetSimulation();
        });
    }

    void InitializeCylinders()
    {
        if (cylinderA && cylinderB)
        {
            // ����A�Ĵ�С��λ�ã���ϸ��
            cylinderA.transform.localScale = new Vector3(cylinderARadius * 2, h / 2, cylinderARadius * 2);
            cylinderA.transform.position = initialPosA;

            // ����B�Ĵ�С��λ�ã��ϴ֣�
            cylinderB.transform.localScale = new Vector3(cylinderBRadius * 2, h / 2, cylinderBRadius * 2);
            cylinderB.transform.position = initialPosB;
        }
    }

    void SetupRigidbodies()
    {
        // ΪA��Ӹ��岢���ó�ʼ״̬
        rbA = cylinderA.GetComponent<Rigidbody>();
        if (rbA == null)
            rbA = cylinderA.AddComponent<Rigidbody>();

        rbA.useGravity = false; // ����UnityĬ��������ʹ���Զ�������
        rbA.collisionDetectionMode = CollisionDetectionMode.Continuous; // ������ײ��⣬��ֹ��͸

        // ΪB��Ӹ��岢���ó�ʼ״̬
        rbB = cylinderB.GetComponent<Rigidbody>();
        if (rbB == null)
            rbB = cylinderB.AddComponent<Rigidbody>();

        rbB.useGravity = false; // ����UnityĬ������
        rbB.isKinematic = true; // ��ʼʱBΪ�˶�ѧ���壬��������Ӱ��
    }

    private void FixedUpdate()
    {
        ApplyCustomGravity();
    }

    void ApplyCustomGravity()
    {
        // ��AӦ������
        if (isFallingA && rbA)
        {
            rbA.velocity += Vector3.down * gravityA * Time.deltaTime;
        }

        // ��BӦ��������������ײ��
        if (isFallingB && rbB)
        {
            rbB.velocity += Vector3.down * gravityB * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        // ���A��B����ײ
        if ((collision.gameObject == cylinderA && gameObject == cylinderB))
        {
            if (!collisionOccured)
            {
                collisionOccured = true;
                isFallingB = true; // B��ʼ��������
                rbB.isKinematic = false; // ����B������ģ��
                collisionStartTime = Time.time;
            }
        }
    }

    void OnTriggerExit(Collider collision)
    {
        // ���A�뿪B����ײ
        if ((collision.gameObject == cylinderA && gameObject == cylinderB))
        {
            if (collisionOccured)
            {
                collisionDuration = Time.time - collisionStartTime;            
            }
        }
    }

    // ����ģ��
    public void ResetSimulation()
    {
        // ��������״̬
        isFallingA = true;
        isFallingB = false;
        collisionOccured = false;
        collisionStartTime = 0f;
        collisionDuration = 0f;
        

        // ��������λ�ú��ٶ�
        cylinderA.transform.position = initialPosA;
        cylinderB.transform.position = initialPosB;

        if (rbA) rbA.velocity = Vector3.zero;
        if (rbB)
        {
            rbB.velocity = Vector3.zero;
            rbB.isKinematic = true;
        }
    }

    public void RefreshSpacing(string spacing)
    {
        initialPosA = new Vector3(0, initialPosB.y + h + float.Parse(spacing), 0);
        activeBtn.gameObject.SetActive(true);
        ResetSimulation();
    }

    // ��Inspector����ʾ��ײʱ��
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 700, 400));
        GUILayout.Label("A����B��ʱ��: " + (collisionDuration > 0 ? collisionDuration.ToString("F3") + "��" : "��δ����"), fontStyle);
        GUILayout.Label("Aλ��: " + cylinderA.transform.position.y.ToString("F2"), fontStyle);
        GUILayout.Label("Bλ��: " + cylinderB.transform.position.y.ToString("F2"), fontStyle);
        GUILayout.Label("AB������ٶ�: " + (rbA.velocity.y - rbB.velocity.y).ToString("F2"), fontStyle);
        GUI.skin.button.fontSize = 50;
        if (GUILayout.Button("����ģ��"))
        {
            ResetSimulation();
        }

        GUILayout.EndArea();
    }
}
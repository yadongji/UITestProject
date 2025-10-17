using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DataRefresh : MonoBehaviour
{
    public TMP_InputField accelerationA;
    public TMP_InputField accelerationB;
    public TMP_InputField vilocityA;
    public TMP_InputField vilocityB;
    public TMP_InputField initialSpacing;
    private Button confirmBtn;
    // Start is called before the first frame update
    void Start()
    {
        confirmBtn = transform.Find("confirmBtn").GetComponent<Button>();
        confirmBtn.onClick.AddListener(RefreshData);
        Time.timeScale = 0f;
    }

    void RefreshData() 
    {
        Time.timeScale = 1f;
        GameObject.Find("CylinderB").GetComponent<FallingModel>().RefreshSpacing(initialSpacing.text);
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassMainView : UINormalBase
{
    [SerializeField] private Button _characterListBtn;
    [SerializeField] private Slider _scoreChinese;  //语文实力
    [SerializeField] private Slider _scoreEnglish;  //英语实力
    [SerializeField] private Slider _scoreMath;     //数学实力
    [SerializeField] private Slider _scorePhysics;  //物理实力
    [SerializeField] private Slider _scoreChemistry;//化学实力
    [SerializeField] private Slider _scoreBiology;  //生物实力


    public override void Init()
    {
        _characterListBtn.onClick.AddListener(()=>DebugHelper.Instance.Log("ssss"));
    }
}

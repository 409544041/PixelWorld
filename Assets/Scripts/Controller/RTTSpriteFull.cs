using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class RTTSpriteFull : MonoBehaviour {

	private SpriteRenderer spriteRenderer = null;
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
		spriteRenderer.material.renderQueue =2980; //��δ���ǳ���Ҫ������������Ҫ���ϣ���Ȼ͸������Ⱦ�㼶�����
	}
}
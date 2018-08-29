using UnityEngine;
using System.Collections;

public class GravityBallEvent : MonoBehaviour {
	//移动速度
	public float speed;
	//重力
	Rigidbody rb;

	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	void Update () 
	{
		//定义方向值
		Vector3 dir = Vector3.zero;
		//获取重力感应器值
		dir.x = Input.acceleration.x;
		dir.z = Input.acceleration.y;
		//如果方向值过大，则将超过的值（X、Y、Z）置为1
		if (dir.sqrMagnitude > 1)
			dir.Normalize ();
		//添加重力
		rb.AddForce (dir * speed * Time.deltaTime, ForceMode.Force);
	}
}

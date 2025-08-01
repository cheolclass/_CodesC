using System;
using System.Collections;
using UnityEngine;

/* Comment colors
	// R.
	// G.
	// B.
	// P. 
	// T.	tomato
	// Y.
	// O.	orange
	// >> 	
	// "#11dea1ff"	
*/

public class Clock : MonoBehaviour
{

    private const float
    hoursToDegrees = -360f / 12f,   // 시/분/초 침의 회전 방향 => 시계 방향(음(-)의 방향)
    // 4 test... ��/��/�� ħ�� ȸ�� ���� => �ð� ����(��(-)�� ����)
                                     
    minutesToDegrees = -360f / 60f,
    secondsToDegrees = -360f / 60f,


    speed = 3.0f,   // swing speed
    swing = 20.0f,  // swing angle
    door_time = 0.8f,
    door_angle = 160f,
    door_speed = door_angle / door_time,

    birdmove_time = 0.2f,
    birdmove_distance = 0.5f,
    birdmove_speed = birdmove_distance / birdmove_time;

    public Transform hours, minutes, seconds, pendlum;
    public Transform door_left, door_right, bird;

    public AudioSource SoundCuckoo;
    public AudioSource SoundOpen;
    public AudioSource SoundClose;

    public bool analog; // 에디터에서 설정 
    public bool trigger;

    // bird movement
    private bool bird_active;
    private int bird_pahse;

    // pendlum & cuckoo 
    private float t;
    private float last_sec;

    void Start()
    {
        t = 0.0f;
        // cuckoo.Play();

        last_sec = 0.0f;
        bird_active = false;
        StartCoroutine(PlayCuckoo()); // 시작시, 뻐꾸기 일단 한 번 보여주고...

        trigger = false;
    }

    void Update()
    {
        float ang;
        DateTime time = DateTime.Now;
        TimeSpan timespan = DateTime.Now.TimeOfDay; // time vs. timespan 차이 

        ang = -90.0f + (float)timespan.TotalHours * hoursToDegrees;
        // 시/분/초 침의 회전 방향 => 시계 방향(음(-)의 방향).
        // 참고) //Y. https://docs.google.com/document/d/1uYqFAUUjCjNlp_FarQmD0w9HNfjt4zXerDZbTLshs4Q/edit?tab=t.0#heading=h.ltuj4acvwlwr 

        hours.localRotation = Quaternion.Euler(ang, 0f, 0f);
        // 시(hour), 사원수, Euler angle의  //B. Gimbal-lock 문제
        // 자신의 Pivot(자신의 원점)을 기준으로 시침을 회전

        ang = -90.0f + (float)timespan.TotalMinutes * minutesToDegrees;
        minutes.localRotation = Quaternion.Euler(ang, 0f, 0f);  // 분

        if (analog) // 침의 움직임이 더 부드럽다
        {
            ang = -90.0f + (float)timespan.TotalSeconds * secondsToDegrees;
            seconds.localRotation = Quaternion.Euler(ang, 0f, 0f);  // 부드러운 초침
        }
        else
        {
            ang = -90.0f + time.Second * secondsToDegrees; //B. DateTime time. 
            seconds.localRotation = Quaternion.Euler(ang, 0f, 0f);  // 한 번에 6도씩 회전하는 초침
        }

        if (time.Second < last_sec || trigger)
        // time.Second < last_sec 이경우는 초침이 59 => 0으로 넘어갈때
        // 즉 매 정각에 뻐꾸기 등장
        // trigger = true; InspV에서 설정
        {
            StartCoroutine(PlayCuckoo());   //p. 뻐꾸기 등장 처리 루틴 호출 
            trigger = false;
        }
        
        last_sec = time.Second;

        // pendlum control
        t += Time.deltaTime * speed; // 회전 각의 크기(Δθ) = 시간(Δt) x 각속도(Δω)

        ang = -90.0f + Mathf.Sin(t) * swing;
        // 시계추 위치: 초기 위치(-90°) + 추 진동값(-1 ~ 1) x 최대 스윙 궤적         
        // Y. //Y. https://docs.google.com/document/d/1uYqFAUUjCjNlp_FarQmD0w9HNfjt4zXerDZbTLshs4Q/edit?tab=t.0#heading=h.35qikxqptuj

        //Debug.Log(">>> ang: " + ang);    //test

        pendlum.localRotation = Quaternion.Euler(ang, 0f, 0f);
        // 자신의 Pivot(자신의 원점, 윗쪽 끝)을 기준으로 시계추를 회전
            //G. -90 ~ -70 ~ -90 ~ -110 ~ -90 ~ 궤적을 반복 
    }

    IEnumerator PlayCuckoo()
    {
        float t;
        Vector3 door_left_rot = door_left.rotation.eulerAngles;
        Vector3 door_right_rot = door_right.rotation.eulerAngles;

        if (!bird_active)
        {
            bird_active = true;

            SoundOpen.Play();
            // 1. open door in 1 sec
            t = 0.0f;
            while (t < door_time)
            {
	    	Debug.Log("Time.deltaTimeTime.deltaTime: "+ Time.deltaTime);
      
                t += Time.deltaTime;
                door_left.Rotate(
                    new Vector3(0f, 0f, Time.deltaTime * door_speed));
                door_right.Rotate(
                    new Vector3(0f, 0f, -Time.deltaTime * door_speed));
                yield return null;
            }
            door_left.rotation =
                Quaternion.Euler(door_left_rot + new Vector3(0f, 0f, door_angle));
            door_right.rotation =
                Quaternion.Euler(door_right_rot + new Vector3(0f, 0f, -door_angle));
            yield return new WaitForSeconds(0.3f);

            // 2. move bird out
            t = 0.0f;
            while (t < birdmove_time)
            {
                t += Time.deltaTime;
                bird.Translate(
                    new Vector3(-Time.deltaTime * birdmove_speed, 0f, 0f));
                yield return null;
            }
            yield return new WaitForSeconds(0.3f);

            // bird sing multiple
            int count = DateTime.Now.Hour;
            if (count == 0)
            {
                count = 12;
            }
            if (count > 12)
            {
                count -= 12;
            }

            for (int i = 0; i < count; i++)
            {
                // 3-1. bird swing
                SoundCuckoo.Play();

                t = 0.0f;
                while (t < 0.4f)
                {
                    t += Time.deltaTime;
                    bird.Rotate(
                        new Vector3(0f, -Time.deltaTime * 80.0f, 0f));
                    yield return null;
                }

                // 3-2. bird swing back
                t = 0.0f;
                while (t < 0.4f)
                {
                    t += Time.deltaTime;
                    bird.Rotate(
                        new Vector3(0f, Time.deltaTime * 80.0f, 0f));
                    yield return null;
                }

                yield return new WaitForSeconds(0.3f);
            }

            // 4. move bird back
            t = 0.0f;
            while (t < birdmove_time)
            {
                t += Time.deltaTime;
                bird.Translate(
                    new Vector3(Time.deltaTime * birdmove_speed, 0f, 0f));
                yield return null;
            }

            yield return new WaitForSeconds(0.3f);

            // Final phase, close door
            SoundClose.Play();
            t = 0.0f;
            while (t < door_time)
            {
                t += Time.deltaTime;
                door_left.Rotate(
                    new Vector3(0f, 0f, -Time.deltaTime * door_speed));
                door_right.Rotate(
                    new Vector3(0f, 0f, Time.deltaTime * door_speed));
                yield return null;
            }
            door_left.rotation = Quaternion.Euler(door_left_rot);
            door_right.rotation = Quaternion.Euler(door_right_rot);

            bird_active = false;
        }
    }
}

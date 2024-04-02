using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Chief : MonoBehaviour
{
    enum States
    {
        Idle,
        Thrust,
        Throw,
        Throw2,
        GenerateBat,
        GenerateBee
    };
    States state;
    Vector3 direction;
    GameObject player1;
    GameObject player2;
    CameraSupport cameraSupport;
    float speed;
    float stateTimer=0f;
    float throwTimer=0f;
    Vector3 idlepos;
    float throwRotation;
    float health=1f;
    public Slider slider;
    public void Hit()
    {
        health-=0.1f;
        slider.value=health;
        if(health<=0f)
        {
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    float alpha;
    public Image image;
    // IEnumerator FadeOut()
    // {
    //     alpha = 0;
    //     // float t = Time.time;
    //     while (alpha < 1)
    //     {
    //         // Debug.Log(Time.deltaTime);
    //         alpha += Time.deltaTime * 0.8f;
    //         // Debug.Log(alpha);
    //         image.color = new Color(0, 0, 0, alpha);
    //         yield return null;
    //     }
    //     // Debug.Log(Time.time - t);
    //     SceneManager.LoadScene("EndScene");
    // }
    void Start()
    {
        player1=GameObject.Find("Player1");
        player2=GameObject.Find("Player2");
        cameraSupport=Camera.main.GetComponent<CameraSupport>();
        idlepos.x = cameraSupport.GetWorldBound().min.x + cameraSupport.GetWorldBound().size.x*0.1f+Random.value * cameraSupport.GetWorldBound().size.x*0.8f;
        idlepos.y = cameraSupport.GetWorldBound().min.y + cameraSupport.GetWorldBound().size.y*0.5f+Random.value * cameraSupport.GetWorldBound().size.y*0.4f;
        idlepos.z=0f;
        speed=3f;
        direction=(idlepos-transform.position).normalized;
        state=States.Idle;
        
    }

    void Update()
    {
        if(player1.GetComponent<Player>().dead)
            return;
        if(player2.GetComponent<Player>().dead)
            return;
        if(Vector3.Distance(player1.transform.position,transform.position)<1.3f)
            player1.GetComponent<Player>().Hit();
        if(Vector3.Distance(player2.transform.position,transform.position)<1.3f)
            player2.GetComponent<Player>().Hit();
        if(state==States.Idle)
        {
            if(Time.time-stateTimer>8f)
                UpdateState();
            transform.position += Time.smoothDeltaTime * speed * direction;
            if(direction.x>=0f)
                GetComponent<SpriteRenderer>().flipX = false;
            else
                GetComponent<SpriteRenderer>().flipX = true;
            if(Vector3.Distance(idlepos,transform.position)<1f)
            {
                idlepos.x = cameraSupport.GetWorldBound().min.x + cameraSupport.GetWorldBound().size.x*0.1f+Random.value * cameraSupport.GetWorldBound().size.x*0.8f;
                idlepos.y = cameraSupport.GetWorldBound().min.y + cameraSupport.GetWorldBound().size.y*0.5f+Random.value * cameraSupport.GetWorldBound().size.y*0.4f;
                direction=(idlepos-transform.position).normalized;
            }
        }
        if(state==States.Thrust)
        {
            if(Time.time-stateTimer>8f)
                UpdateState();
            transform.position += Time.smoothDeltaTime * speed * direction;
            if(direction.x>=0f)
                GetComponent<SpriteRenderer>().flipX = false;
            else
                GetComponent<SpriteRenderer>().flipX = true;
            Bounds myBound = GetComponent<Renderer>().bounds;
            CameraSupport.WorldBoundStatus status = cameraSupport.CollideWorldBound(myBound);
            if (status != CameraSupport.WorldBoundStatus.Inside)
            {
                var cameraShake = FindObjectOfType<CameraShaker>();
                // 使用 CameraShaker 震动相机
                cameraShake.Shake();
                if(Random.value>0.5)
                    direction = (player1.transform.position - transform.position).normalized;
                else
                    direction = (player2.transform.position - transform.position).normalized;
            }
        }
        else if(state==States.Throw)
        {
            if(Time.time-throwTimer>0.2f)
            {
                GameObject banana = Instantiate(Resources.Load("Prefabs/Banana") as GameObject);
                banana.transform.position = transform.position;
                banana.transform.rotation = Quaternion.Euler(0f,0f,throwRotation);
                banana = Instantiate(Resources.Load("Prefabs/Banana") as GameObject);
                banana.transform.position = transform.position;
                banana.transform.rotation = Quaternion.Euler(0f,0f,360f-throwRotation);
                throwTimer=Time.time;
            }
            if(throwRotation>330f)
                UpdateState();
            else
                throwRotation+=Time.smoothDeltaTime*100f;
        }
        else if(state==States.Throw2)
        {
            for(int i=0;i<3;i++)
            {
                Vector3 throwPos=new Vector3();
                throwPos.x = cameraSupport.GetWorldBound().min.x + cameraSupport.GetWorldBound().size.x*0.1f+Random.value * cameraSupport.GetWorldBound().size.x*0.8f;
                throwPos.y = cameraSupport.GetWorldBound().min.y + cameraSupport.GetWorldBound().size.y*0.1f+Random.value * cameraSupport.GetWorldBound().size.y*0.8f;
                throwPos.z=0f;
                for(int j=0;j<8;j++)
                {
                    GameObject banana = Instantiate(Resources.Load("Prefabs/Banana") as GameObject);
                    banana.transform.position = throwPos;
                    banana.transform.rotation = Quaternion.Euler(0f,0f,j*45f);
                }
            }
            UpdateState();
        }
        else if(state==States.GenerateBat)
        {
            GameObject bat = Instantiate(Resources.Load("Prefabs/Bat") as GameObject);
            bat.transform.position = transform.position;
            UpdateState();
        }
        else if(state==States.GenerateBee)
        {
            GameObject bee = Instantiate(Resources.Load("Prefabs/Bee") as GameObject);
            bee.transform.position =new Vector3(cameraSupport.GetWorldBound().min.x + cameraSupport.GetWorldBound().size.x*0.5f,cameraSupport.GetWorldBound().min.y + cameraSupport.GetWorldBound().size.y*0.9f,0f);
            UpdateState();
        }
    }

    void UpdateState()
    {
        if(state==States.Idle)
        {
            int randomNumber = Random.Range(1,6);
            if(randomNumber==1)
                toThrust();
            else if(randomNumber==2)
                toThrow();
            else
                state=(States)randomNumber;
        }
        else
        {
            idlepos.x = cameraSupport.GetWorldBound().min.x + cameraSupport.GetWorldBound().size.x*0.1f+Random.value * cameraSupport.GetWorldBound().size.x*0.8f;
            idlepos.y = cameraSupport.GetWorldBound().min.y + cameraSupport.GetWorldBound().size.y*0.5f+Random.value * cameraSupport.GetWorldBound().size.y*0.4f;
            speed=3f;
            direction=(idlepos-transform.position).normalized;
            state=States.Idle;
        }
        stateTimer=Time.time;
    }

    void toThrust()
    {
        if(Random.value>0.5)
            direction=(player1.transform.position-transform.position).normalized;
        else
            direction=(player2.transform.position-transform.position).normalized;
        speed=25f;
        state=States.Thrust;
    }
    void toThrow()
    {
        throwRotation=30f;
        state=States.Throw;
    }
}

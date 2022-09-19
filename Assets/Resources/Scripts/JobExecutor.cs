using System;
using System.Collections.Generic;
using UnityEngine;

public class JobExecutor : MonoBehaviour{
    public static JobExecutor wkr;
    private Queue<Action> jobs = new();

    void Awake() {
        wkr = this;
    }

    void Update() {
        while (jobs.Count > 0) 
            jobs.Dequeue().Invoke();
    }

    public void addJob(Action newJob) {
        jobs.Enqueue(newJob);
    }
}

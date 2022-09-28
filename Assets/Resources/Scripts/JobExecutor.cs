using System;
using System.Collections.Generic;
using UnityEngine;

public class JobExecutor : MonoBehaviour {
    public static JobExecutor wkr;
    private readonly Queue<Action> jobs = new();

    private void Awake() {
        wkr = this;
    }

    private void Update() {
        while (jobs.Count > 0)
            jobs.Dequeue().Invoke();
    }

    public void addJob(Action newJob) {
        jobs.Enqueue(newJob);
    }
}
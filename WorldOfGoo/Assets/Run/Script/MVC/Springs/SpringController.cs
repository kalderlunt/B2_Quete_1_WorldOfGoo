using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController
{
    private SpringModel model;
    private SpringView view;

    public SpringController(SpringModel model, SpringView view)
    {
        this.model = model;
        this.view = view;
    }

    public void UpdateSpring()
    {
        ApplySpringForce();
        view.UpdateView();
    }

    private void ApplySpringForce()
    {
        Vector2 direction = model.BallB.Rigidbody.position - model.BallA.Rigidbody.position;

        float currentLength = direction.magnitude;
        float stretch = currentLength - model.NaturalLength;

        // Calcul de la force du ressort
        float springConstant = 0.5f;
        Vector2 force = direction.normalized * stretch * springConstant; // 0.5f constante de ressort

        // Applique la force sur les deux boules connectées
        model.BallA.Rigidbody.AddForce(force);
        model.BallB.Rigidbody.AddForce(-force);
    }
}
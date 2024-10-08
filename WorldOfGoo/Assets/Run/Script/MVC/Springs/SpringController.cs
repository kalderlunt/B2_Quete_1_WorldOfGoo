using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController : MonoBehaviour
{
    private SpringModel model;
    private SpringView view;

    private SpringController(SpringModel model, SpringView view)
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
        Vector2 direction = model.BallB.Position - model.BallA.Position;

        float currentLength = direction.magnitude;
        float stretch = currentLength - model.NaturalLength;

        // Calcul de la force du ressort
        Vector2 force = direction.normalized * stretch * 0.5f; // 0.5f constante de ressort

        // Applique la force sur les deux boules connectées
        model.BallA.Velocity += force / model.BallA.Mass;
        model.BallB.Velocity -= force / model.BallB.Mass;
    }
}
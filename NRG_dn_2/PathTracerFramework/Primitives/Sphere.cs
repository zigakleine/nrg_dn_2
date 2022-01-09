using System;
using MathNet.Numerics.Integration;

namespace PathTracer
{
  class Sphere : Shape
  {
    public double Radius { get; set; }
    public Sphere(double radius, Transform objectToWorld)
    {
      Radius = radius;
      ObjectToWorld = objectToWorld;
    }

    public override (double?, SurfaceInteraction) Intersect(Ray ray)
    {
      Ray r = WorldToObject.Apply(ray);

      // TODO: Compute quadratic sphere coefficients

      // TODO: Initialize _double_ ray coordinate values
      double ox = r.o.x; double oy = r.o.y; double oz = r.o.z;
      double dx = r.d.x; double dy = r.d.y; double dz = r.d.z;
      double a = dx * dx + dy * dy + dz * dz;
      double b = 2 * (dx * ox + dy * oy + dz * oz);
      double c = ox * ox + oy * oy + oz * oz - Radius * Radius;


      // TODO: Solve quadratic equation for _t_ values
      (bool isSolution, double t0, double  t1) = Utils.Quadratic(a, b, c);
      if(!isSolution)
      {
        return (null, null);
      }

      // TODO: Check quadric shape _t0_ and _t1_ for nearest intersection
      if (t1 <= 0) return (null, null);
      double tShapeHit = t0;
      if (tShapeHit <= 0) {
        tShapeHit = t1;
      }

      // TODO: Compute sphere hit position and $\phi$
      Vector3 pHit = r.Point(tShapeHit);

      // TODO: Return shape hit and surface interaction
      Vector3 normal = pHit.Clone().Normalize();
      Vector3 wo = -r.d;
      Vector3 dpdu = new Vector3(-pHit.y, pHit.x, 0);
      SurfaceInteraction interection = new SurfaceInteraction(pHit,normal, wo, dpdu, this);

      return (tShapeHit, ObjectToWorld.Apply(interection));

      }

    public override (SurfaceInteraction, double) Sample()
    {
      // TODO: Implement Sphere sampling
      Vector3 pObj = new Vector3(0, 0, 0) + Radius * Samplers.UniformSampleSphere();
     
      // TODO: Return surface interaction and pdf
      Vector3 n = ObjectToWorld.ApplyNormal(pObj);

      bool reverseOrientation = false;
      if(reverseOrientation) { 
        n *= -1; 
      }
      pObj *= Radius / pObj.Length(); 
      Vector3 dpdu = new Vector3(-pObj.y, pObj.x, 0);
      double pdf = 1 / Area();

      return (ObjectToWorld.Apply(new SurfaceInteraction(pObj, n, Vector3.ZeroVector, dpdu, this)), pdf);
    }

    public override double Area() { return 4 * Math.PI * Radius * Radius; }

    public override double Pdf(SurfaceInteraction si, Vector3 wi)
    {
      throw new NotImplementedException();
    }

  }
}

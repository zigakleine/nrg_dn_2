using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PathTracer.Samplers;

namespace PathTracer
{
    class PathTracer
    {
        public Spectrum Li(Ray r, Scene s)
        {
            Spectrum L = Spectrum.ZeroSpectrum;
            Spectrum beta = Spectrum.Create(1.0);


            int maxBounces = 20;
            for(int nBounces = 0; nBounces<maxBounces; nBounces++)
            {

                (double? distance, SurfaceInteraction intersection) = s.Intersect(r);
                if(!distance.HasValue)
                {
                    break;
                }

                Vector3 wo = -r.d;

                if (intersection.Obj is Light)
                {
                    if(nBounces == 0)
                    {
                        // direct light hit
                        L = L.AddTo(beta * intersection.Le(wo));
                    }
                    break;
                }


                // adding from light source
                Spectrum Ld = Light.UniformSampleOneLight(intersection, s);
                L = L.AddTo(beta * Ld);

                //Spectrum f = isect.bsdf->Sample_f(wo, &wi, sampler.Get2D(), &pdf, BSDF_ALL, &flags);
                Shape obj = (Shape) intersection.Obj;
                (Spectrum f, Vector3 wi, double pdf, bool isSpecular) = obj.BSDF.Sample_f(wo, intersection);
                r = intersection.SpawnRay(wi);
                //beta *= f * AbsDot(wi, isect.shading.n) / pdf;
                beta *= f * Vector3.AbsDot(wi, intersection.Normal) / pdf;


                if(nBounces > 3)
                {
                    double q = 1 - beta.c.Max();
                    if (ThreadSafeRandom.NextDouble() < q)
                    {
                        break;
                    }
                    beta = beta / (1 - q);
                }

            }

          
            return L;


        }

    }
}

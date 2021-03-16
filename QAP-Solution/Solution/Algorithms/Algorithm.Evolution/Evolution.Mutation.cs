﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Solution
{
    public partial class EvolutionAlgorithm
    {
        protected List<Individ> MUTATION(List<Individ> src, int M_SIZEi = 0, int M_TYPEi = 0, double M_CHANCEi = 1, int M_SALT_SIZEi = 4)
        {
            Random rand = new Random();
            int mutationCounter = 0;
            List<Individ> aResult=new List<Individ>(src.ToArray());
            List<int> aMutatedIndividsId = new List<int>();
            int size = (src.Count < M_SIZEi) ? src.Count : M_SIZEi;
            while (mutationCounter < size)
            {
                int iRnd = rand.Next(src.Count);
                if (aMutatedIndividsId.Contains(iRnd) == false && M_CHANCEi >= rand.Next(101))
                {
                    switch (M_TYPEi)
                    {
                        case 0:
                            aResult[iRnd]._mutationSaltation(M_SALT_SIZEi);
                            break;
                        case 1:
                            aResult[iRnd]._mutationDot();
                            break;
                    }
                    mutationCounter++;
                    aMutatedIndividsId.Add(iRnd);
                }
            }
            return aResult;
        }
    }
}
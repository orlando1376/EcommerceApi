﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.DTOs
{
    public class DireccionDTO
    {
        public string Calle { get; set; }
        public string Ciudad { get; set; }
        public string Departamento { get; set; }
        public string CodigoPostal { get; set; }
        public string Pais { get; set; }
    }
}

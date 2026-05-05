using ServicioPrestamo.Domain.Common;
using ServicioPrestamo.Domain.Entities;
using System.Collections.Generic;

namespace ServicioPrestamo.Application.Interfaces;

public interface IPrestamoServicio
{
    IEnumerable<Prestamo> Select();
    Result Create(Prestamo prestamo);
    Result Update(Prestamo prestamo);
    Result Delete(Prestamo prestamo);
    Prestamo? GetById(int id);
    Result ValidarPrestamo(Prestamo prestamo);
    int CountPrestamosActivos(int lectorId);
    int InsertAndReturnId(Prestamo prestamo);
}

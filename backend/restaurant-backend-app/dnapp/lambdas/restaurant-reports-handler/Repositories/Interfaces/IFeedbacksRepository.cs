using Function.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Repositories.Interfaces
{
    public interface IFeedbacksRepository
    {
        public Task<List<Feedback>> GetServiceFeedbacks(string reservationId);
    }
}

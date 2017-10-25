﻿using Jambo.Domain.Exceptions;
using Jambo.Domain.Model.Schools;
using Jambo.Domain.Model.Schools.Events;
using MediatR;
using System;

namespace Jambo.Consumer.Application.DomainEventHandlers.Schools
{
    public class ChildPickedUpDomainEventDomainEventHandler : IRequestHandler<ChildPickedUpDomainEvent>
    {
        private readonly ISchoolReadOnlyRepository schoolReadOnlyRepository;
        private readonly ISchoolWriteOnlyRepository schoolWriteOnlyRepository;

        public ChildPickedUpDomainEventDomainEventHandler(
            ISchoolReadOnlyRepository schoolReadOnlyRepository,
            ISchoolWriteOnlyRepository schoolWriteOnlyRepository)
        {
            this.schoolReadOnlyRepository = schoolReadOnlyRepository ??
                throw new ArgumentNullException(nameof(schoolReadOnlyRepository));
            this.schoolWriteOnlyRepository = schoolWriteOnlyRepository ??
                throw new ArgumentNullException(nameof(schoolWriteOnlyRepository));
        }

        public void Handle(ChildPickedUpDomainEvent domainEvent)
        {
            School school = schoolReadOnlyRepository.GetSchool(domainEvent.AggregateRootId).Result;

            if (school.Version != domainEvent.Version)
                throw new TransactionConflictException(school, domainEvent);

            school.Apply(domainEvent);
            schoolWriteOnlyRepository.UpdateSchool(school).Wait();
        }
    }
}
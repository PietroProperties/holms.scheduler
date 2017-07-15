HOLMS Task Scheduler
====================

The HOLMS task scheduler is a Windows service that runs scheduled tasks for
the HOLMS hotel property management system. It calls into the HOLMS
microservices using gRPC, on a schedule controlled by Quartz.

After we built this, we discovered Heroku's article on custom clock processes,
which more or less describes exactly what this is, even using our same choice
of tools:

https://devcenter.heroku.com/articles/scheduled-jobs-custom-clock-processes

Please let us know if you find this helpful! hello@shortbar.com


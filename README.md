# EF and WebAPI REST services.
traditionally i would have done a single commit to a separate (non "main") branch followed by a merge to main as to keep a proper workflow. that did not seem to be appropriate or required for this exercise.
Not much logic is included in the source at the moment hence no tests. the majority of the functionality is being handled by ef and webapi.
There could be some tests around ensuring a csv is being parsed appropriately and key integrity however this did not seem to be appropriate for the exercise given there are only a few lines of code there.
The self referential data related to an employees manager and how the collection is populated could possibly be done a bit cleaner (less code, less internal selects) using other features in ef however i would have to spend some more time to research and find out how this is done using ef core.

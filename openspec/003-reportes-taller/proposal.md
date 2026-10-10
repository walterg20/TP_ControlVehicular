# Proposal: Workshop Job Management and Reports (Mechanic)

## Background
Currently, mechanics do not have a streamlined way to record the parts used in their services or to view reports of the work they have completed. Furthermore, there is a risk of exposing other mechanics' work details if access controls are not strictly enforced on these reports.

## Goals
- Allow mechanics to add spare parts (`DetalleServicio`) to the services they are working on.
- Provide a "Performed Services" report that allows mechanics to view their own work history.
- Ensure strict data isolation: Mechanics can only see their own work, while Administrators can view the work of any mechanic.

## Non-Goals
- Full inventory management for spare parts (this is handled in a separate module).
- Payroll or commission calculations based on the services performed.

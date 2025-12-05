# 📄 Product Requirements Document (PRD)  
## **LienWorksSharp – Phase 1 (Local-Only MVP for Blazor)**

**Goal:** Build an ASP.NET Core **Blazor** application implementing structured workflow for lien research, templates, clients, and work orders.  
**Phase-1 Storage:** JSON + local filesystem  
**Primary Goal:** Clean maintainable UI with scalable architecture for future automation.

---

## 🔹 Global Architectural Standards

### ✔ All UI pages must follow this structure:

| File Type | Purpose |
|---|---|
| `*.razor` | UI markup only — components, layout, binding targets |
| `*.razor.cs` | All logic, event handlers, service calls, state operations |

---

### Application Title  
**LienWorksSharp**

---

## 1. Application Scope

| Module | Purpose |
|---|---|
| Global Templates | Manage master copies of Owner Report / NoF / CoL templates |
| Clients | Store client records, contacts, and override templates |
| Work Orders | Create, track, and expand into job-level detail |
| File Store | Local JSON + file system storage in `/LienWorks_Data` |

---

## 2. Storage Model

| Type | Location |
|---|---|
| Entities (Clients, Work Orders, Jobs, Contacts…) | JSON files |
| Document files | `/LienWorks_Data/...` folder layout |
| Future DB support | Must be direct-swap capable |

---

## 3. Global Templates (UI Requirements)

Page Route: `/templates/global`  
Menu Label: **Templates → Global Templates**

### Document Types

| Display Name |
|---|
| Owner Report |
| Notice of Furnishing |
| Claim of Lien |

### UI Behavior

| Element | Rule |
|---|---|
| Show current template + download | Always visible |
| Upload/Replace action | Must open modal dialog |
| History view | Modal dialog — hidden until opened |
| History grid columns | Filename, Upload Timestamp |

---

## 4. Clients (UI Requirements)

Page Route: `/clients`  
Menu Label: **Clients**

### Clients List Page

| Requirement | Description |
|---|---|
| Display as table/grid | One row per client |
| Add Client | Button → opens modal dialog |
| Double-click row | Opens Client Detail page |
| Manage button per row | Navigates to details `/clients/{id}` |

#### Grid Columns

| Column |
|---|
| Client Name |
| Address |
| Default Required Documents |
| Primary Contact Name |
| Client Template Indicators + Download |
| Manage Button |

---

## Client Detail Page (`/clients/{id}`)

### Must Support

| Action | UI Mechanism |
|---|---|
| Edit general client details | Form always visible |
| Upload/Replace templates | Modal dialog |
| View template history | Modal dialog (grid view) |
| Manage contacts | Grid + Add/Edit/Delete modals |

Only one Primary Contact allowed — set/unset automatically.

---

## 5. Work Orders (Default Page)

Default Route: `/workorders`  
Menu Label: **Work Orders**

### Work Orders Grid

| Column |
|---|
| Client Name |
| Work Order Name |
| Supplied/Created Date |
| Status (badge) |

Sorted newest → oldest.

### Job Expansion

Rows expand to sub-grid displaying:

| Job Column |
|---|
| Address |
| County |
| Owner |
| Required Documents |
| Status Badge |

---

## 6. Research + Document Prep (Deferred to Phase-2)

| Must Keep | Must Hide for now |
|---|---|
| Backend logic | Frontend workflow pages |
| Job status transitions | Research UI visibility |
| File storage routines | Navigation links |

---

## 7. UI/UX Styling Requirements

| Element | Requirement |
|---|---|
| Tables/Grids | Used for Work Orders + Jobs + Clients |
| Modals | Required for Add/Upload/Edit |
| Status badges | Color-coded clearly |
| Sidebar/Top Nav | Only Templates / Clients / Work Orders |
| Default Landing Page | Must be **Work Orders** |

---

## 8. Future Expansion

| Future Feature | Prepared For |
|---|---|
| SQL database | Replace JSON backend |
| Authentication & roles | Admin template management |
| Google Maps API | Auto county extraction |
| Playwright/Puppeteer | Web research automation |
| Document Generation | Auto-fill Owner/NoF/CoL |

---

## MVP Completion Checklist

- Work Orders as landing page  
- Global Templates page w/ modal uploads + history restore  
- Clients grid + modal add + detail page functionality  
- Contact management complete  
- `.razor` contains UI only — `.razor.cs` contains logic  
- Research workflow logic retained but UI hidden  


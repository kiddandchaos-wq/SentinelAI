<img width="1254" height="1254" alt="image" src="https://github.com/user-attachments/assets/f33d52b7-192b-4517-8b9a-2d93d5d50464" />

# Sentinel AI

**Intelligence for how organizations actually operate.**

Sentinel AI is an organizational intelligence and governance platform that helps organizations capture, search, and protect institutional knowledge. Upload SOPs and policy documents as PDFs, and Sentinel AI extracts and chunks the text into a searchable repository, secured with JWT authentication, role-based access control, and a full audit trail.

**Why it exists:** when experts leave or documents go stale, organizations lose knowledge without noticing. Sentinel AI is being built to surface that risk before it becomes a business problem.

---

## Current Features

**Security**
- JWT authentication
- Password hashing
- Role-based access control
- Rate limiting

**Audit & Governance**
- Audit logging
- Risk scoring
- Policy management

**Knowledge Management**
- Document upload and PDF ingestion
- Document metadata storage and file path tracking
- PDF text extraction and knowledge chunking
- Searchable knowledge repository

**Knowledge Search**
- Keyword search endpoint
- Question retrieval endpoint
- Document and chunk retrieval

---

## Technology Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core, .NET 9 |
| Data access | Entity Framework Core |
| Database | PostgreSQL |
| PDF processing | PdfPig |

---

## Knowledge Pipeline

```
Upload Document
      ↓
KnowledgeDocuments
      ↓
PDF Extraction
      ↓
KnowledgeChunks
      ↓
Search
      ↓
Question Retrieval
```

---

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/knowledge` | System status |
| GET | `/api/knowledge/documents` | Returns stored documents |
| GET | `/api/knowledge/chunks` | Returns extracted knowledge chunks |
| GET | `/api/knowledge/search?q=` | Searches extracted document content |
| GET | `/api/knowledge/ask?q=` | Question retrieval |
| POST | `/api/knowledge/upload` | Uploads and processes a PDF |

---

## Roadmap

Sentinel AI is designed as a modular platform. Start with one module and add more over time. The core services above (security, audit, and knowledge processing) are shared by every module.

| Module | Focus | Planned features | Status |
|---|---|---|---|
| **Sentinel Atlas** | SOP and knowledge intelligence | Document ownership tracking, knowledge citations, confidence scoring, duplicate SOP detection, stale document detection | Next up |
| **Sentinel Nexus** | Knowledge ownership intelligence | Expert mapping, knowledge concentration analysis, retirement risk detection | Planned |
| **Sentinel Flow** | Operational friction analytics | Workflow bottleneck detection, process analytics, organizational health scoring | Planned |

---

## Vision

Sentinel AI is evolving into an operating platform that helps organizations see what others miss: which procedures are outdated, who holds critical knowledge, and where work slows down, before those gaps become business problems.

---

## Screenshots

See the [`screenshots`](./screenshots) folder.


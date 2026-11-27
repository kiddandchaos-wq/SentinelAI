# SentinelAI

SentinelAI is an organizational intelligence and governance platform designed to help organizations manage knowledge, improve operational visibility, and prepare for AI-powered decision support.

## Current Features

### Security

- JWT Authentication
- Password Hashing
- Role-Based Access Control
- Rate Limiting

### Audit & Governance

- Audit Logging
- Risk Scoring
- Policy Management

### Knowledge Management

- Document Upload
- PDF Ingestion
- Document Metadata Storage
- File Path Tracking

### Knowledge Processing

- PDF Text Extraction
- Knowledge Chunking
- Searchable Knowledge Repository

### Knowledge Search

- Search Endpoint
- Question Endpoint
- Document Retrieval

## Technology Stack

### Backend

- ASP.NET Core
- .NET 9
- Entity Framework Core

### Database

- PostgreSQL

### Knowledge Processing

- PdfPig
- Knowledge Chunking

## Knowledge Pipeline

```text
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

## Current API Endpoints

### Knowledge

```http
GET /api/knowledge
```

System status.

```http
GET /api/knowledge/documents
```

Returns stored documents.

```http
GET /api/knowledge/chunks
```

Returns extracted knowledge chunks.

```http
GET /api/knowledge/search?q=
```

Searches extracted document content.

```http
GET /api/knowledge/ask?q=
```

Question retrieval endpoint.

```http
POST /api/knowledge/upload
```

Uploads and processes PDF documents.

## Roadmap

### Sentinel Atlas

Knowledge and SOP Intelligence

Planned Features:

- Document Ownership Tracking
- Knowledge Citations
- Confidence Scoring
- Duplicate SOP Detection
- Stale Document Detection

### Sentinel Nexus

Knowledge Ownership Intelligence

Planned Features:

- Expert Mapping
- Knowledge Concentration Analysis
- Retirement Risk Detection

### Sentinel Flow

Operational Friction Analytics

Planned Features:

- Workflow Bottleneck Detection
- Process Analytics
- Organizational Health Scoring

## Vision

SentinelAI is evolving into an organizational intelligence platform that helps organizations identify knowledge loss, operational risk, process friction, and governance gaps before they become business problems.
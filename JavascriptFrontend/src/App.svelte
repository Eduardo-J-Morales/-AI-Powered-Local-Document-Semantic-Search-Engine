<script lang="ts">
	import { onMount } from 'svelte';
	let documents: any[] = [];
	let selectedFile: File | null = null;
	let searchQuery = '';
	let selectedDoc: any = null;
	let tagInput = '';
	let isUploading = false;
	let aiSuggestedTags: string[] = [];
	let userTags: string[] = [];

	onMount(async () => {
		await fetchDocuments();
	});

	async function fetchDocuments() {
		const res = await fetch('/api/documents');
		documents = await res.json();
	}

	async function handleFileChange(event: Event) {
		const files = (event.target as HTMLInputElement).files;
		if (files && files.length > 0) {
			selectedFile = files[0];
		}
	}

	async function uploadDocument() {
		if (!selectedFile) return;
		isUploading = true;
		const formData = new FormData();
		formData.append('file', selectedFile);

		const res = await fetch('/api/documents/upload', {
			method: 'POST',
			body: formData
		});
		isUploading = false;
		selectedFile = null;
		await fetchDocuments();
	}

	function selectDocument(doc: any) {
		selectedDoc = doc;
		userTags = [...(doc.tags || [])];
		aiSuggestedTags = doc.aiSuggestedTags || [];
	}

	async function searchDocuments() {
		const res = await fetch(`/api/documents/search?query=${encodeURIComponent(searchQuery)}`);
		documents = await res.json();
		selectedDoc = null;
	}

	async function addTag() {
		if (!tagInput.trim() || !selectedDoc) return;
		const res = await fetch(`/api/documents/${selectedDoc.id}/tags`, {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ tag: tagInput.trim() })
		});
		if (res.ok) {
			userTags.push(tagInput.trim());
			tagInput = '';
		}
	}

	async function acceptSuggestedTag(tag: string) {
		if (!selectedDoc) return;
		const res = await fetch(`/api/documents/${selectedDoc.id}/tags`, {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ tag })
		});
		if (res.ok) {
			userTags.push(tag);
			aiSuggestedTags = aiSuggestedTags.filter(t => t !== tag);
		}
	}

	function previewContent(doc: any) {
		if (doc.preview) return doc.preview;
		if (doc.fileName.endsWith('.pdf')) return 'PDF preview not available.';
		if (doc.fileName.endsWith('.txt')) return doc.content?.slice(0, 500) || '';
		return 'Preview not available.';
	}
</script>

<main>
	<h1>AI Document Organizer</h1>

	<section>
		<h2>Upload Document</h2>
		<input type="file" accept=".pdf,.txt" on:change={handleFileChange} />
		<button on:click={uploadDocument} disabled={!selectedFile || isUploading}>
			{isUploading ? 'Uploading...' : 'Upload'}
		</button>
	</section>

	<section>
		<h2>Search Documents</h2>
		<input
			type="text"
			placeholder="Search by keyword or tag"
			bind:value={searchQuery}
			on:keydown={(e) => e.key === 'Enter' && searchDocuments()}
		/>
		<button on:click={searchDocuments}>Search</button>
	</section>

	<section>
		<h2>Document List</h2>
		{#if documents.length === 0}
			<p>No documents found.</p>
		{:else}
			<ul>
				{#each documents as doc}
					<li>
						<a href="#" on:click|preventDefault={() => selectDocument(doc)}>
							{doc.fileName}
						</a>
						{#if doc.tags && doc.tags.length}
							<span>Tags: {doc.tags.join(', ')}</span>
						{/if}
					</li>
				{/each}
			</ul>
		{/if}
	</section>

	{#if selectedDoc}
		<section>
			<h2>Document Details</h2>
			<p><strong>Name:</strong> {selectedDoc.fileName}</p>
			<p><strong>Path:</strong> {selectedDoc.filePath}</p>
			<p><strong>Tags:</strong> {userTags.join(', ') || 'None'}</p>
			<div>
				<h3>Add Tag</h3>
				<input type="text" placeholder="New tag" bind:value={tagInput} />
				<button on:click={addTag}>Add</button>
			</div>
			{#if aiSuggestedTags.length}
				<div>
					<h3>AI Suggested Tags</h3>
					<ul>
						{#each aiSuggestedTags as tag}
							<li>
								{tag}
								<button on:click={() => acceptSuggestedTag(tag)}>Accept</button>
							</li>
						{/each}
					</ul>
				</div>
			{/if}
			<div>
				<h3>Preview</h3>
				<pre>{previewContent(selectedDoc)}</pre>
			</div>
		</section>
	{/if}
</main>

<style>
main {
	max-width: 800px;
	margin: 2rem auto;
	padding: 2rem;
	background: #fff;
	border-radius: 8px;
	box-shadow: 0 2px 8px rgba(0,0,0,0.1);
	font-family: system-ui, sans-serif;
}
section {
	margin-bottom: 2rem;
}
input[type="file"] {
	margin-right: 1rem;
}
ul {
	list-style: none;
	padding: 0;
}
li {
	margin-bottom: 0.5rem;
}
a {
	color: #007acc;
	text-decoration: none;
	cursor: pointer;
}
a:hover {
	text-decoration: underline;
}
button {
	margin-left: 0.5rem;
}
pre {
	background: #f4f4f4;
	padding: 1rem;
	border-radius: 4px;
	overflow-x: auto;
}
</style>
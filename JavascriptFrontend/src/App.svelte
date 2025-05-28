<script lang="ts">
	import { onMount } from 'svelte';
	let filesInfo: typeof fileInfo[] = []
	
	onMount(async () => {
		await fetchDocuments()
	})

	async function fetchDocuments() {
		try {
			const response = await fetch('http://localhost:5191/api/documents');
			if (!response.ok) throw Error("Connection error with the api");
			const data = await response.json();
			// Inicializa el array de tags si no existe
			filesInfo = data.map(f => ({ ...f, tags: f.tags ?? [] }));
		} catch (e) {
			console.log(e);
		}
	}

	let selectedFile: File | null = null

	let fileInfo = {
		filename: '',
		contentType: '',
		size: 0,
		route: '',
		tags: [] as string[]
	}

	async function postFileMetadata(metadata) {
		try {
			const response = await fetch('http://localhost:5191/api/documents/upload', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify(metadata)
			});
			if (!response.ok) {
				const error = await response.json();
				console.error('API error:', error.message);
			}
		} catch (e) {
			console.error('Network error:', e);
		}
	}

	function handleFileChange(event) {
		const files = (event.target as HTMLInputElement).files;
		if (files && files.length > 0) {
			selectedFile = files[0]
			fileInfo = {
				filename: selectedFile.name,
				contentType: selectedFile.type,
				size: selectedFile.size,
				route: (selectedFile as any).webkitRelativePath || '(not available)',
				tags: []
			}
			filesInfo = [...filesInfo, { ...fileInfo }]
			postFileMetadata(fileInfo)
			event.target.value = null
		}
	}

	async function deleteFile(file) {
		try {
			const res = await fetch(`http://localhost:5191/api/documents/${file}`, { method: "DELETE" })
			if (!res.ok) {
				const error = await response.json();
				console.error('API error:', error.message);
			}
		} catch (e) {
			console.log(e)
		}
		await fetchDocuments()
	}

	// --- Modal y lógica de tags ---
	let showModal = false;
	let currentFileIdx: number | null = null;
	let tagInput = '';

	function openTagModal(idx: number) {
		currentFileIdx = idx;
		tagInput = '';
		showModal = true;
	}

	function closeModal() {
		showModal = false;
		currentFileIdx = null;
		tagInput = '';
	}

	async function addTag() {
		if (currentFileIdx === null || !tagInput.trim()) return;
		const tags = tagInput.split(',').map(t => t.trim()).filter(Boolean);
		// Actualiza el array local
		filesInfo[currentFileIdx].tags = [...(filesInfo[currentFileIdx].tags || []), ...tags];
		// Envía cada tag al endpoint
		for (const tag of tags) {
			await fetch('http://localhost:5191/api/documents/tag', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({
					filename: filesInfo[currentFileIdx].filename,
					tag
				})
			});
		}
		tagInput = '';
		closeModal();
	}

	function formatSize(size: number): string {
		return (size / (1024 * 1024)).toFixed(2) + ' MB';
	}
</script>

<main>
	<section>
		<h2>Upload File</h2>
		<input type="file" id="file-upload" class="hidden-file-input" on:change={handleFileChange} />
		<label for="file-upload" class="file-upload-label">
			Select File
		</label>
	</section>

	{#if filesInfo.length > 0}
	<section>
		<h3>Files</h3>
		<div class="card-container">
			{#each filesInfo as info, idx}
				<div class="file-card">
					<div class="file-card-header">
						<span class="file-name">{info.filename}</span>
						<div class="file-card-actions">
							<button class="icon-btn" on:click={() => deleteFile(info.filename)}>❌</button>
							<button class="icon-btn" on:click={() => openTagModal(idx)}>🏷️</button>
						</div>
					</div>
					<div class="file-card-body">
						<div><strong>Content Type:</strong> {info.contentType}</div>
						<div><strong>Size:</strong> {info.size} bytes</div>
						<div><strong>Route:</strong> {info.route}</div>
						{#if info.tags && info.tags.length > 0}
							<div>
								<strong>Tags:</strong>
								{#each info.tags as tag}
									<span class="tag">{tag}</span>
								{/each}
							</div>
						{/if}
					</div>
				</div>
			{/each}
		</div>
	</section>
	{/if}

	{#if showModal}
		<div class="modal-backdrop" on:click={closeModal}></div>
		<div class="modal">
			<h4>Add tags (separate with commas)</h4>
			<input type="text" bind:value={tagInput} placeholder="tag1, tag2, tag3" on:keydown={(e) => e.stopPropagation()} />
			<div class="modal-actions">
				<button on:click={addTag}>Add</button>
				<button on:click={closeModal}>Cancel</button>
			</div>
		</div>
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

.hidden-file-input {
	display: none;
}

.file-upload-label {
	display: inline-block;
	padding: 0.5rem 1.5rem;
	background: #007acc;
	color: #fff;
	border-radius: 4px;
	cursor: pointer;
	font-weight: bold;
	transition: background 0.2s;
}
.file-upload-label:hover {
	background: #005fa3;
}

.card-container {
	display: flex;
	flex-wrap: wrap;
	gap: 1rem;
}

.file-card {
	background: #f8fafd;
	border: 1px solid #e0e6ed;
	border-radius: 8px;
	box-shadow: 0 1px 4px rgba(0,0,0,0.04);
	padding: 1rem 1.5rem;
	min-width: 220px;
	max-width: 300px;
	flex: 1 1 220px;
	display: flex;
	flex-direction: column;
	justify-content: space-between;
}

.file-card-header {
	display: flex;
	align-items: center;
	justify-content: space-between;
	margin-bottom: 0.5rem;
}

.file-card-actions {
	display: flex;
	gap: 0.5rem;
}

.icon-btn {
	background: none;
	border: none;
	cursor: pointer;
	font-size: 1.2rem;
	padding: 0 0.3rem;
	transition: color 0.2s;
}
.icon-btn:hover {
	color: #007acc;
}

.file-name {
	font-weight: bold;
	font-size: 1.1rem;
	overflow: hidden;
	text-overflow: ellipsis;
	white-space: nowrap;
	max-width: 170px;
}

.file-card-body > div {
	margin-bottom: 0.25rem;
	font-size: 0.97rem;
}

.tag {
	display: inline-block;
	background: #e0e6ed;
	color: #333;
	border-radius: 3px;
	padding: 2px 8px;
	margin-right: 4px;
	font-size: 0.9em;
}

.modal-backdrop {
	position: fixed;
	top: 0; left: 0; right: 0; bottom: 0;
	background: rgba(0,0,0,0.3);
	z-index: 10;
}

.modal {
	position: fixed;
	top: 50%; left: 50%;
	transform: translate(-50%, -50%);
	background: #fff;
	padding: 2rem;
	border-radius: 8px;
	box-shadow: 0 2px 16px rgba(0,0,0,0.2);
	z-index: 11;
	min-width: 300px;
}

.modal-actions {
	margin-top: 1rem;
	display: flex;
	gap: 1rem;
	justify-content: flex-end;
}
</style>
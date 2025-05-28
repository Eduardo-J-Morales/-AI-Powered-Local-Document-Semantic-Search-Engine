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
						<div><strong>Size:</strong> {formatSize(info.size)}</div>
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
:root {
    --primary: #2563eb;
    --primary-dark: #1e40af;
    --bg: #f4f6fb;
    --card-bg: #fff;
    --border: #e5e7eb;
    --shadow: 0 4px 24px rgba(30, 64, 175, 0.08);
    --radius: 14px;
    --tag-bg: #e0e7ff;
    --tag-color: #3730a3;
    --danger: #ef4444;
    --danger-bg: #fee2e2;
    --modal-bg: #fff;
    --modal-shadow: 0 8px 32px rgba(30, 41, 59, 0.18);
}

body {
    background: var(--bg);
}

main {
    max-width: 900px;
    margin: 2.5rem auto;
    padding: 2.5rem 2rem;
    background: var(--card-bg);
    border-radius: var(--radius);
    box-shadow: var(--shadow);
    font-family: 'Inter', system-ui, sans-serif;
}

h2, h3, h4 {
    font-weight: 700;
    color: var(--primary-dark);
    letter-spacing: -0.5px;
    margin-bottom: 1rem;
}

section {
    margin-bottom: 2.5rem;
}

.hidden-file-input {
    display: none;
}

.file-upload-label {
    display: inline-block;
    padding: 0.7rem 2rem;
    background: var(--primary);
    color: #fff;
    border-radius: var(--radius);
    cursor: pointer;
    font-weight: 600;
    font-size: 1.08rem;
    letter-spacing: 0.02em;
    box-shadow: 0 2px 8px rgba(37,99,235,0.08);
    transition: background 0.18s, box-shadow 0.18s, transform 0.1s;
    border: none;
}
.file-upload-label:hover {
    background: var(--primary-dark);
    box-shadow: 0 4px 16px rgba(37,99,235,0.12);
    transform: translateY(-2px) scale(1.03);
}

.card-container {
    display: flex;
    flex-wrap: wrap;
    gap: 1.5rem;
}

.file-card {
    background: var(--card-bg);
    border: 1.5px solid var(--border);
    border-radius: var(--radius);
    box-shadow: var(--shadow);
    padding: 1.3rem 1.7rem;
    min-width: 240px;
    max-width: 320px;
    flex: 1 1 240px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    transition: box-shadow 0.18s, border 0.18s, transform 0.12s;
}
.file-card:hover {
    box-shadow: 0 8px 32px rgba(37,99,235,0.12);
    border-color: var(--primary);
    transform: translateY(-2px) scale(1.02);
}

.file-card-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 0.7rem;
}

.file-card-actions {
    display: flex;
	flex-direction: column;
    gap: 0.7rem;
}

.icon-btn {
    background: none;
    border: none;
    cursor: pointer;
    font-size: 1.3rem;
    padding: 0.2rem 0.5rem;
    border-radius: 6px;
    transition: background 0.16s, color 0.16s, transform 0.1s;
}
.icon-btn:hover {
    background: var(--tag-bg);
    color: var(--primary-dark);
    transform: scale(1.15);
}
.icon-btn:last-child:hover {
    background: var(--danger-bg);
    color: var(--danger);
}

.file-name {
	margin-top: -50px;
    font-weight: 600;
    font-size: 1.08rem;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    max-width: 170px;
    color: var(--primary-dark);
}

.file-card-body > div {
    margin-bottom: 0.8rem;
    font-size: 1rem;
    color: #374151;
}

.file-card-body:is(:nth-child(1), :nth-child(2), :nth-child(3)) {
	margin-top: -40px ;
}


.tag {
    display: inline-block;
    background: var(--tag-bg);
    color: var(--tag-color);
    border-radius: 6px;
    padding: 3px 10px;
    margin-right: 6px;
    font-size: 0.93em;
    font-weight: 500;
    letter-spacing: 0.01em;
    margin-bottom: 2px;
}

.modal-backdrop {
    position: fixed;
    top: 0; left: 0; right: 0; bottom: 0;
    background: rgba(30,41,59,0.18);
    z-index: 10;
    backdrop-filter: blur(2px);
}

.modal {
    position: fixed;
    top: 50%; left: 50%;
    transform: translate(-50%, -50%);
    background: var(--modal-bg);
    padding: 2.2rem 2.5rem;
    border-radius: var(--radius);
    box-shadow: var(--modal-shadow);
    z-index: 11;
    min-width: 340px;
    display: flex;
    flex-direction: column;
    align-items: stretch;
}

.modal h4 {
    margin-bottom: 1.1rem;
    font-size: 1.18rem;
}

.modal input[type="text"] {
    padding: 0.7rem 1rem;
    border-radius: 7px;
    border: 1.5px solid var(--border);
    font-size: 1.05rem;
    margin-bottom: 1.2rem;
    transition: border 0.16s;
}
.modal input[type="text"]:focus {
    border-color: var(--primary);
    outline: none;
}

.modal-actions {
    margin-top: 0.5rem;
    display: flex;
    gap: 1.2rem;
    justify-content: flex-end;
}

.modal-actions button {
    padding: 0.6rem 1.5rem;
    border-radius: 7px;
    border: none;
    font-weight: 600;
    font-size: 1.01rem;
    cursor: pointer;
    transition: background 0.16s, color 0.16s, transform 0.1s;
    background: var(--primary);
    color: #fff;
}
.modal-actions button:hover {
    background: var(--primary-dark);
    transform: scale(1.04);
}
.modal-actions button:last-child {
    background: #e5e7eb;
    color: #374151;
}
.modal-actions button:last-child:hover {
    background: #d1d5db;
    color: #1e293b;
}
</style>
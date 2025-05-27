<script lang="ts">
	import { onMount } from 'svelte';

	onMount(async () => {
	try {
		const response = await fetch('http://localhost:5191/api/documents');
			
		if (!response.ok) throw Error("Connection error with the api");
		
		const data = await response.json();
		console.log(data);
	} catch (e) {
		console.log(e);
	}
	})

	let selectedFile: File | null = null
	let fileInfo = {
		name: '',
		content_type: '',
		size: 0,
		route: ''
	}

	let filesInfo: typeof fileInfo[] = []

	function handleFileChange(event) {
		const files = (event.target as HTMLInputElement).files;
		if (files && files.length > 0) {
			selectedFile = files[0]
			fileInfo = {
				name : selectedFile.name,
				content_type : selectedFile.type,
				size : selectedFile.size,
				route : (selectedFile as any).webkitRelativePath || '(not available)'
			}

			filesInfo = [...filesInfo, { ...fileInfo }]

			// Optionally clear the file input visually (if using bind:this, not needed here)
			event.target.value = null
		}
	}
</script>

<main>
	<section>
		<h2>Upload Document</h2>
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
						<span class="file-index">#{idx + 1}</span>
						<span class="file-name">{info.name}</span>
					</div>
					<div class="file-card-body">
						<div><strong>Content Type:</strong> {info.content_type}</div>
						<div><strong>Size:</strong> {info.size} bytes</div>
						<div><strong>Route:</strong> {info.route}</div>
					</div>
				</div>
			{/each}
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
	margin-bottom: 0.5rem;
}

.file-index {
	background: #007acc;
	color: #fff;
	border-radius: 50%;
	width: 1.5rem;
	height: 1.5rem;
	display: flex;
	align-items: center;
	justify-content: center;
	font-size: 1rem;
	margin-right: 0.75rem;
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
</style>
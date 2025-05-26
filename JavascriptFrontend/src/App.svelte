<script lang="ts">
	import { onMount } from 'svelte';

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

			event.target.files[0] = null
			
		}
	}

</script>

<main>
	<section>
		<h2>Upload Document</h2>
		<input type="file" id="file-upload" class="hidden-file-input" on:change={handleFileChange(event)} />
		<label for="file-upload" class="file-upload-label">
			Select File
		</label>
	</section>

{#if filesInfo.length > 0 } 
	<section>
		<h3>Files</h3>
		<ul>
			{#each filesInfo as info, idx}
			<li>
				<strong>{idx + 1}</strong>
				Name: { info.name} |
				Content Type: {info.content_type} |
				Size: {info.size} bytes |
				Route: {info.route}
			</li>
		{/each}	
		</ul>
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

</style>
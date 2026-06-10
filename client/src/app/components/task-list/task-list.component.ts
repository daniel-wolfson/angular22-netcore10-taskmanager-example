import { DatePipe } from '@angular/common';
import { Component, computed, input, output, signal } from '@angular/core';
import { TaskItem } from '../../models/task-item.model';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
})
export class TaskListComponent {
  readonly tasks = input.required<TaskItem[]>();
  readonly loading = input<boolean>(false);
  readonly errorMessage = input<string | null>(null);
  readonly markDone = output<string>();

  readonly search = signal('');

  // Filters tasks by title and sorts them by created date (newest first)
  readonly filteredTasks = computed(() => {
    const term = this.search().trim().toLowerCase();

    return this.tasks()
      .filter((task) => task.title.toLowerCase().includes(term))
      .sort((a, b) => new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime());
  });

  // Updates the search term signal
  // when the user types in the search input
  setSearch(value: string): void {
    this.search.set(value);
  }

  // Emits the markDone event with the task ID
  // when the user clicks the "Mark as Done" button
  markAsDone(id: string): void {
    this.markDone.emit(id);
  }
}

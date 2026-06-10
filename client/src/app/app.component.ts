import { Component, computed, inject, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { catchError, of } from 'rxjs';
import { CreateTaskComponent } from './components/create-task/create-task.component';
import { TaskListComponent } from './components/task-list/task-list.component';
import { TaskItem } from './models/task-item.model';
import { TaskService } from './services/task.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CreateTaskComponent, TaskListComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  private readonly taskService = inject(TaskService);
  private readonly errorSignal = signal<string | null>(null);

  private readonly tasksResource = rxResource<TaskItem[], void>({
    stream: () =>
      this.taskService.getTasks().pipe(
        catchError((error) => {
          console.error('Error loading tasks:', error);
          // Extract error message and set it in signal
          let errorMessage = 'An unexpected error occurred';
          if (error && typeof error === 'object' && 'status' in error) {
            errorMessage = `Error: Loading tasks from server failed`;
          } else if (error && typeof error === 'object' && 'message' in error) {
            errorMessage = error.message;
          }

          this.errorSignal.set(errorMessage);
          return of([]); // Return empty array to prevent error state
        }),
      ),
  });

  readonly tasks = computed(() => this.tasksResource.value() ?? []);
  readonly loading = this.tasksResource.isLoading;
  readonly errorMessage = this.errorSignal.asReadonly();

  handleCreated(task: TaskItem): void {
    this.tasksResource.update((tasks) => [task, ...(tasks ?? [])]);
  }

  handleMarkDone(id: string): void {
    this.taskService.markAsDone(id).subscribe((updatedTask) => {
      this.tasksResource.update((tasks) =>
        (tasks ?? []).map((task) => (task.id === updatedTask.id ? updatedTask : task)),
      );
    });
  }
}

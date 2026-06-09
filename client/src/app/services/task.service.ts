import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { TaskItem } from '../models/task-item.model';

export interface CreateTaskRequest {
  title: string;
  description?: string;
}

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly http = inject(HttpClient);

  getTasks(title?: string): Observable<TaskItem[]> {
    const params = title ? new HttpParams().set('title', title) : undefined;
    return this.http.get<TaskItem[]>('/api/tasks', { params });
  }

  createTask(req: CreateTaskRequest): Observable<TaskItem> {
    return this.http.post<TaskItem>('/api/tasks', req);
  }

  markAsDone(id: string): Observable<TaskItem> {
    return this.http.patch<TaskItem>(`/api/tasks/${id}/status`, {});
  }
}

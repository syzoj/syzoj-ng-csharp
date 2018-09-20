import React, { Component } from 'react';
import { Link } from 'react-router-dom';

export class Problemset extends Component {
  displayName = "题库"

  constructor(props) {
    super(props);
    this.state = { problems: [], loading: true };

    fetch('api/problemset/cd6363d3-ca44-4c81-b660-15db225a91cc/problems')
      .then(response => response.json())
      .then(data => {
        this.setState({ problems: data.Problems, loading: false });
      });
  }

  static renderProblemset(problems) {
    return (
      <table className='table table-bordered table-hover table-striped'>
        <thead>
          <tr>
            <th className="text-center" style={{width: '5em'}}>ID</th>
            <th>标题</th>
          </tr>
        </thead>
        <tbody>
          {problems.map(problem =>
            <tr className="text-center" key={problem.ProblemId}>
              <td>{problem.ProblemsetProblemId}</td>
              <td className="text-left"><Link to={"/problem/" + problem.ProblemsetProblemId}>{problem.Title}</Link></td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Problemset.renderProblemset(this.state.problems);

    return (
      <div>
        {contents}
      </div>
    );
  }
}
